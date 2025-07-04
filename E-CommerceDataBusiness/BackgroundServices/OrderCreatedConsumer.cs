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
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;

namespace E_CommerceDataBusiness.BackgroundServices
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly IRabbitMQService _rabbitmqService;
        private readonly IServiceProvider _serviceProvider;
        //private readonly IUserRepository  _userRepository;
        private readonly IEmailService _emailService;

        public OrderCreatedConsumer(IRabbitMQService rabbitmqService, IServiceProvider serviceProvider,  IEmailService emailService)
        {
            _rabbitmqService = rabbitmqService;
            _serviceProvider = serviceProvider;
            //_userRepository = userRepository;
            _emailService = emailService;
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

                                var user = await dbContext.Users.FindAsync(existingOrder.UserId);
                                await _emailService.SendEmailAsync(user.Email, " // Created New Order //", $"Hello {user.UserName} Your Order with Id {existingOrder.OrderId} has  Complated Staust");


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
