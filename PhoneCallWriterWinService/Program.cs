using PhoneCallWriterWinService.Kafka;
using System;
using System.Configuration;
using System.ServiceProcess;

namespace PhoneCallWriterWinService
{
    public static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        public static void Main()
        {
#if !DEBUG
            // Тестирование в режиме DEBUG (проверка записи сообщений в топик)
            var kafkaProducer = new KafkaProducer(ConfigurationManager.AppSettings["TopicName"]);
            for (var i = 0; i < 100; i++)
                kafkaProducer.Execute(Guid.NewGuid().ToString());
            kafkaProducer.Dispose();
#else
            ServiceBase.Run(new ServiceBase[]
            {
                new WinService()
            });
#endif
        }
    }
}