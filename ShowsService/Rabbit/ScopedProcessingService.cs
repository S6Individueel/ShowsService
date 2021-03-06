using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShowsService.DTOs;
using ShowsService.Extensions;
using ShowsService.Rabbit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShowsService.Rabbit
{
    internal class ScopedProcessingService : IScopedProcessingService
    {
        private int executionCount = 0;
        private readonly double hoursTillUpdate = 500;
        private readonly ILogger _logger;
        private readonly IDistributedCache cache;

        public ScopedProcessingService(ILogger<ScopedProcessingService> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            cache = distributedCache;
        }
        public async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                Console.WriteLine("Sleeping for Rabbit...");
                await Task.Delay(TimeSpan.FromSeconds(20));
                _logger.LogInformation(
                "Scoped Processing Service is working for SHOWSSERVICE");
                var factory = new ConnectionFactory()
                {
                    HostName = "rabbitmq",
                    Port = 5672,
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(15)
                };
                IConnection connection = factory.CreateConnection();
                IModel channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: "topic_exchange", type: "topic");         //EXCHANGE creation

                var queueName = channel.QueueDeclare().QueueName;                       //QUEUE creation with random name

                channel.QueueBind(queue: queueName,
                                  exchange: "topic_exchange",
                                  routingKey: "shows.*.trending");                    //BINDING creation

                Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>                                     //MESSAGE RECEIVING HANDLER
                {
                    Console.WriteLine("[!!!RECEIVING RECEIVING!!!]");
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    List<ShowDTO> trendingShows = JsonConvert.DeserializeObject<List<ShowDTO>>(message);//TODO: Alles  behalve title en url is null maar in message niet???

                    await cache.SetShowAsync<String>(trendingShows[0].Media_type.GenerateKey(), message, TimeSpan.FromHours(24), TimeSpan.FromHours(24));


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
            await Task.Delay(TimeSpan.FromHours(hoursTillUpdate), stoppingToken);
        }
    }
}
