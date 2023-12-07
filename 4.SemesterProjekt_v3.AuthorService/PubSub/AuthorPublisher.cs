using RabbitMQ.Client;
using System.Text;

namespace _4.SemesterProjekt_v3.AuthorService.PubSub
{
    public class AuthorPublisher
    {
        public void PublishToMessageQueue(string integrationEvent, string eventData)
        {
            var factory = new ConnectionFactory();
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(eventData);
            channel.BasicPublish(exchange: "author",
                routingKey: integrationEvent,
                basicProperties: null,
                body: body);
        }
    }
}
