/*using ShowsService.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShowsService.Extensions;
using ShowsService.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShowsService.Rabbit
{
    public class RabbitConsumer : BackgroundService
    {
        private readonly ILogger<RabbitConsumer> _logger;
        private readonly IDistributedCache cache;
        public RabbitConsumer(ILogger<RabbitConsumer> logger, IDistributedCache distributedCache)
        {
            cache = distributedCache;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"GracePeriodManagerService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"GracePeriod task doing background work.");

                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)

                await Task.Run(() => UpdateInfo(), stoppingToken);
            }

            _logger.LogDebug($"GracePeriod background task is stopping.");
        }

        public void UpdateInfo()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_exchange", type: "topic");         //EXCHANGE creation

                var queueName = channel.QueueDeclare().QueueName;                       //QUEUE creation with random name

                channel.QueueBind(queue: queueName,
                                  exchange: "topic_exchange",
                                  routingKey: "shows.*.trending");                    //BINDING creation

                Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>                                     //MESSAGE RECEIVING HANDLER
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    List<ShowDTO> trendingShows = JsonConvert.DeserializeObject<List<ShowDTO>>(message);//TODO: Alles  behalve title en url is null maar in message niet???

                    await cache.SetShowAsync<String>(trendingShows[0].Media_type.GenerateKey(), message, TimeSpan.FromHours(12), TimeSpan.FromHours(12));

                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                      routingKey,
                                      message);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
*/