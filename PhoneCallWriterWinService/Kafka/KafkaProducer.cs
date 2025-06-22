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

        private readonly IProducer<Null, string> _producer;
        private readonly string _topicName;

        public KafkaProducer(string topicName)
        {
            try
            {
                _topicName = topicName;
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

        /// <summary>
        /// Делаем все это синхронно.
        /// Если в будущем будут большие объемы данных,
        /// перейдем на async/await, tasks.
        /// </summary>
        public void Execute(string message)
        {
            _producer.Produce(_topicName, new Message<Null, string> { Value = message }, (dr) =>
            {
                if (!dr.Error.IsError)
                    _logger.Info($"Сообщение '{message}' отправлено в топик '{_topicName}'");
                else
                    _logger.Error($"Не удалось записать сообщение '{message}' в топик '{_topicName}': {dr.Error.Reason}");
            });
            _producer.Flush();
        }

        public void Dispose() => _producer.Dispose();
    }
}