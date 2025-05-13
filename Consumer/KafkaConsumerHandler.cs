using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace product.service.Consumer
{
    public class KafkaConsumerHandler : BackgroundService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly string topicName = "productTopic";

        public KafkaConsumerHandler(ConsumerConfig consumerConfig)
        {
            _consumerConfig = consumerConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int retryCount = 0;
            while (retryCount < 5)
            {
                try
                {
                    using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
                    consumer.Subscribe(topicName);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = consumer.Consume(stoppingToken);
                        var product = JsonSerializer.Deserialize<Product>(result.Message.Value);
                        Console.WriteLine($"Received: {product}");
                    }
                }
                catch (KafkaException ex)
                {
                    Console.WriteLine($"Kafka connection failed: {ex.Message}");
                    retryCount++;
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
        



        /*{

            using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig)
                .SetErrorHandler((_, ex) => Console.WriteLine(ex.Reason))
                .Build())
            {
                consumer.Subscribe(topicName);
                try
                {
                    while(true)
                    {
                        var consumerResult = consumer.Consume();
                        var product = JsonSerializer.Deserialize<Product>(consumerResult.Message.Value);
                        Console.WriteLine($" Product has been received from apigateway service: => {product!.ToString()}");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.InnerException!.Message);
                    throw ex;
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
        */
    }
}