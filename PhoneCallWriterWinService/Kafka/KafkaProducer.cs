using Confluent.Kafka;
using NLog;
using System;
using System.Configuration;

namespace PhoneCallWriterWinService.Kafka
{
    public class KafkaProducer : IDisposable
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ProducerConfig _config = new ProducerConfig
        {
            BootstrapServers = ConfigurationManager.AppSettings["BootstrapServers"].ToString()
        };

        private const int FLUSH_SEC = 2;
        private readonly IProducer<Null, string> _producer;
        private readonly string _topicName = ConfigurationManager.AppSettings["TopicName"];

        public KafkaProducer()
        {
            _producer = new ProducerBuilder<Null, string>(_config).Build();
        }

        public void Write(string message)
        {
            try
            {
                _producer.Produce(_topicName, new Message<Null, string> { Value = message }, (dr) =>
                {
                    if (dr.Error.Code == ErrorCode.NoError)
                        _logger.Info($"Записана строка '{dr.Value}' в топик '{dr.Topic}'");
                    else
                        _logger.Error($"Ошибка отправки строки '{dr.Value}' в топик '{dr.Topic}': {dr.Error.Reason}");
                });
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                _producer.Flush(TimeSpan.FromSeconds(FLUSH_SEC));
            }
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}