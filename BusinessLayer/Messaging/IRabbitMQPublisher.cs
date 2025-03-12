using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Messaging
{
    public interface IRabbitMQPublisher
    {
        void PublishMessage<T>(T message, string routingKey);
    }
}
