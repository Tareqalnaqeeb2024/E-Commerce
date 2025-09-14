using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.BackgroundServices
{
    public class WelcomeEmailMessage
    {
        public string Email { get; set; }
        public string UserName { get; set; }

        public WelcomeEmailMessage() { }

        public WelcomeEmailMessage(string email, string userName)
        {
            Email = email;
            UserName = userName;
        }
    }

    public class WelcomeEmailConsumer : BackgroundService
    {
        private readonly IRabbitMQService _rabbitmqService;
        private readonly IEmailService _emailService;

        public WelcomeEmailConsumer(IRabbitMQService rabbitmqService, IEmailService emailService)
        {
            _rabbitmqService = rabbitmqService;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "welcome-email-queue",
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
                        var json = Encoding.UTF8.GetString(body);
                        var message = System.Text.Json.JsonSerializer.Deserialize<WelcomeEmailMessage>(json);

                        if (message != null && !string.IsNullOrEmpty(message.Email))
                        {
                            await _emailService.SendEmailAsync(
                                message.Email,
                                "Welcome to Our Platform",
                                $"Hello {message.UserName},\n\nYour account has been successfully created!");
                 
                        }

                        // تأكيد استلام الرسالة
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ خطأ أثناء معالجة البريد الترحيبي: {ex}");
                        // إعادة الرسالة للـ Queue
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                channel.BasicConsume(
                    queue: "welcome-email-queue",
                    autoAck: false,
                    consumer: consumer
                );

                // إبقاء الخدمة مستمرة
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }


    }


}
