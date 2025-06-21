using PhoneCallWriterWinService.Kafka;
using System;
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
#if DEBUG
            // Тестирование 
            var kafkaProducer = new KafkaProducer();
            for (var i = 0; i < 100; i++)
                kafkaProducer.Write(Guid.NewGuid().ToString());
            kafkaProducer.Dispose();
#else
            ServiceBase.Run(new ServiceBase[]
            {
                new CrmCallsKafkaWriter()
            });
#endif
        }
    }
}