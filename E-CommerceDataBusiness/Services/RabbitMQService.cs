//using E_CommerceDataBusiness.Interfaces;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Text;
//using System.Text.Json;

//namespace E_CommerceDataBusiness.Services
//{
//    public class RabbitMQService : IRabbitMQService, IDisposable
//    {
//        private readonly IConnection _connection;
//        private readonly IModel _channel;

//        public RabbitMQService(IConnectionFactory factory)
//        {
//            _connection = factory.CreateConnection();
//            _channel = _connection.CreateModel();
//        }

//        public void PublishMessage<T>(T message, string queueName)
//        {
//            _channel.QueueDeclare(queue: queueName,
//                                 durable: true,
//                                 exclusive: false,
//                                 autoDelete: false,
//                                 arguments: null);

//            var json = JsonSerializer.Serialize(message);
//            var body = Encoding.UTF8.GetBytes(json);

//            var properties = _channel.CreateBasicProperties();
//            properties.Persistent = true;

//            _channel.BasicPublish(exchange: "",
//                                 routingKey: queueName,
//                                 basicProperties: properties,
//                                 body: body);
//        }

//        public void ConsumeMessage<T>(string queueName, Func<T, Task> onMessageReceived)
//        {
//            _channel.QueueDeclare(queue: queueName,
//                                 durable: true,
//                                 exclusive: false,
//                                 autoDelete: false,
//                                 arguments: null);

//            var consumer = new AsyncEventingBasicConsumer(_channel);
//            consumer.Received += async (model, ea) =>
//            {
//                var body = ea.Body.ToArray();
//                var json = Encoding.UTF8.GetString(body);
//                var message = JsonSerializer.Deserialize<T>(json);

//                try
//                {
//                    if (message != null)
//                    {
//                        await onMessageReceived(message);
//                        _channel.BasicAck(ea.DeliveryTag, multiple: false); // تأكيد الاستلام
//                    }
//                    else
//                    {
//                        _channel.BasicNack(ea.DeliveryTag, false, false); // تجاهل الرسالة
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"❌ خطأ أثناء المعالجة: {ex}");
//                    _channel.BasicNack(ea.DeliveryTag, false, true);
//                }

//            };

//            _channel.BasicConsume(queue: queueName,
//                                 autoAck: false,
//                                 consumer: consumer);
//        }

//        public void Dispose()
//        {
//            _channel?.Close();
//            _connection?.Close();
//        }
//    }
//}
