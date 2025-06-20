using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces
{
    public interface IRabbitMQService
    {
        void PublishMessage<T>(T message, string queueName);
        void ConsumeMessage<T>(string queueName, Func<T, Task> onMessageReceived);
    }
}
