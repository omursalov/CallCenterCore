using Confluent.Kafka;
using NLog;
using System;
using System.Configuration;
using System.IO;

namespace PhoneCallWriterWinService.Kafka
{
    public class KafkaProducer : IDisposable
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ProducerConfig _config = new ProducerConfig
        {
            BootstrapServers = ConfigurationManager.AppSettings["BootstrapServers"]
        };

        private const int FLUSH_SEC = 2;
        private readonly IProducer<Null, string> _producer;
        private readonly string _topicName = ConfigurationManager.AppSettings["TopicName"];

        public KafkaProducer()
        {
            try
            {
                _producer = new ProducerBuilder<Null, string>(_config).Build();
            }
            catch (DllNotFoundException)
            {
                if (!Library.IsLoaded)
                {
                    var pathToLibrd = System.IO.Path.Combine(
                        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                        $"librdkafka\\x86\\librdkafka.dll");
                    Library.Load(pathToLibrd);
                    _producer = new ProducerBuilder<Null, string>(_config).Build();
                }
            }
        }

        public void Write(string message)
        {
            try
            {
                var x = _producer.ProduceAsync(_topicName, new Message<Null, string> { Value = message }).Result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                _producer.Flush(TimeSpan.FromSeconds(FLUSH_SEC));
            }
        }

        public void Dispose() => _producer.Dispose();
    }
}