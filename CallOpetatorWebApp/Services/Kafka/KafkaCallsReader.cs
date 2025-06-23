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
                GroupId = "console-consumer-81101",
                AutoOffsetReset = AutoOffsetReset.Earliest
            }).Build();
            _consumer.Subscribe(configuration["Kafka:TopicName"]);
        }

        public KafkaOutCall Next()
        {
            ConsumeResult<Null, string> result = null;

            lock (_lockObj)
            {
                result = _consumer.Consume();
            }
            
            return JsonConvert.DeserializeObject<KafkaOutCall>(result.Message.Value);
        }

        public void Dispose() => _consumer.Dispose();
    }
}