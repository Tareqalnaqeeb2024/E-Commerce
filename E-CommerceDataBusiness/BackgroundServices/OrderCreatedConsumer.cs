using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using E_CommerceDataAccess.Models;
using Microsoft.Extensions.DependencyInjection;
using E_CommerceDataAccess.Data;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;

namespace E_CommerceDataBusiness.BackgroundServices
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly IRabbitMQService _rabbitmqService;
        private readonly IServiceProvider _serviceProvider;

        public OrderCreatedConsumer(IRabbitMQService rabbitmqService, IServiceProvider serviceProvider)
        {
            _rabbitmqService = rabbitmqService;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "orders",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var order = JsonConvert.DeserializeObject<Order>(Encoding.UTF8.GetString(body));

                        // Process the order (e.g., update status to "Completed")
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            var existingOrder = await dbContext.Orders.FindAsync(order.OrderId);
                            if (existingOrder != null)
                            {
                                existingOrder.Status = "Completed";
                                await dbContext.SaveChangesAsync();
                                //_logger.LogInformation($"Processed Order: {order.OrderId}");
                            }
                        }

                        // Manually acknowledge the message
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "Error processing order");
                        // Requeue the message if processing fails
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                channel.BasicConsume(
                    queue: "orders",
                    autoAck: false, // Manual acknowledgment
                    consumer: consumer
                );

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }

        }

    }
}
