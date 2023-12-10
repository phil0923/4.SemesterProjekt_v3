using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using _4.SemesterProjekt_v3ArticleService.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Newtonsoft.Json.Linq;

namespace _4.SemesterProjekt_v3ArticleService.PubSub
{
    public static class AuthorSubscriber
    {
        public static void ListenForIntegrationEvents()
        {
            var factory = new ConnectionFactory();
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var contextOptions = new DbContextOptionsBuilder<ArticleDbContext>()
                    .UseSqlite(@"Data Source=ArticleDb.db")
                    .Options;
                var dbContext = new ArticleDbContext(contextOptions);

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                var data = JObject.Parse(message);
                var type = ea.RoutingKey;
                if (type == "author.add")
                {
                    if (dbContext.Author.Any(x => x.Id == data["id"].Value<int>()))
                    {
                        Console.WriteLine("Ignoring old/duplicate entity");
                    }
                    else
                    {
                        dbContext.Author.Add(new Entities.Author()
                        {
                            Id = data["id"].Value<int>(),
                            Name = data["name"].Value<string>(),
                            Version = data["version"].Value<int>()
                        });
                        dbContext.SaveChanges();
                    }
                }
                else if (type == "author.update")
                {
                    int newVersion = data["version"].Value<int>();
                    var author = dbContext.Author.First(a => a.Id == data["id"].Value<int>());
                    if (author.Version >= newVersion)
                    {
                        Console.WriteLine("Ignoring old/duplicate entity");
                    }
                    else
                    {
                        author.Name = data["newname"].Value<string>();
                        author.Version = newVersion;
                        dbContext.SaveChanges();
                    }
                }
                channel.BasicAck(ea.DeliveryTag, false);
            };
            channel.BasicConsume(
                queue: "author.articleservice",
                autoAck: false,
                consumer: consumer);
        }
    }
}
