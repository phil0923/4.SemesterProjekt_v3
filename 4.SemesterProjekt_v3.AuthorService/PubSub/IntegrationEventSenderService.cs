
using _4.SemesterProjekt_v3.AuthorService.Data;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RabbitMQ.Client;
using System.Text;

namespace _4.SemesterProjekt_v3.AuthorService.PubSub
{
    public class IntegrationEventSenderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private CancellationTokenSource _wakeupCancelationTokenSource = new CancellationTokenSource();

        public IntegrationEventSenderService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            using var scope = _scopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<AuthorDbContext>();
            dbContext.Database.EnsureCreated();
        }

        public void StartPublishingOutstandingIntegrationEvents()
        {
            _wakeupCancelationTokenSource.Cancel();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PublishOutstandingIntegrationEvents(stoppingToken);
            }
        }

        private async Task PublishOutstandingIntegrationEvents(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory();
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();
                channel.ConfirmSelect(); // Enable publisher confirms
                IBasicProperties props = channel.CreateBasicProperties();
                props.DeliveryMode = 2; // Persist message

                while (!stoppingToken.IsCancellationRequested)
                {
                    {
                        using var scope = _scopeFactory.CreateScope();
                        using var dbContext = scope.ServiceProvider.GetRequiredService<AuthorDbContext>();
                        var events = dbContext.IntegrationEvent.OrderBy(x => x.Id).ToList();
                        foreach (var e in events)
                        {
                            var body = Encoding.UTF8.GetBytes(e.Data);
                            channel.BasicPublish(
                                exchange: "author",
                                routingKey: e.Event,
                                basicProperties: null,
                                body: body);
                            Console.WriteLine("Published: " + e.Event + " " + e.Data);
                            dbContext.Remove(e);
                            dbContext.SaveChanges();
                        }
                    }

                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_wakeupCancelationTokenSource.Token, stoppingToken);
                    try
                    {
                        await Task.Delay(Timeout.Infinite, linkedCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        if (_wakeupCancelationTokenSource.Token.IsCancellationRequested)
                        {
                            Console.WriteLine("Publish requested");
                            var temp = _wakeupCancelationTokenSource;
                            _wakeupCancelationTokenSource = new CancellationTokenSource();
                            temp.Dispose();
                        }
                        else if (stoppingToken.IsCancellationRequested)
                        {
                            Console.WriteLine("Shutting down");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
