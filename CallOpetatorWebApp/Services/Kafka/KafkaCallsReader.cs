using CallOpetatorWebApp.Models;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace CallOpetatorWebApp.Services.Kafka
{
    /// <summary>
    /// Буду лочить ОДИН consumer, может и криво, 
    /// но в будущем можно увеличить кол-во consumers
    /// </summary>
    public class KafkaCallsReader : IKafkaCallsReader
    {
        private IConsumer<Null, string> _consumer;
        private object _lockObj = new object();
        private readonly ConsumerConfig _config;

        /// <summary>
        /// SINGLETON
        /// </summary>
        public KafkaCallsReader(IConfiguration configuration) 
        {
            _consumer = new ConsumerBuilder<Null, string>(new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            }).Build();
            _consumer.Subscribe(configuration["Kafka:TopicName"]);
        }

        /// <summary>
        /// Может вернуть NULL (нет звонков в Kafka, кончились)
        /// </summary>
        /// <returns></returns>
        public KafkaOutCall Next()
        {
            ConsumeResult<Null, string> result = null;


            /*while (true)
            {
                try
                {
                    var cr = _consumer.Consume();

                    Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occurred: {e.Error.Reason}");

                }
            }*/


            lock (_lockObj)
            {
                var TIMEOUT_SEC = 5;
                result = _consumer.Consume(TimeSpan.FromSeconds(TIMEOUT_SEC));
            }
            
            return result != null ? JsonConvert.DeserializeObject<KafkaOutCall>(result.Message.Value) : null;
        }

        public void Dispose() => _consumer.Dispose();
    }
}