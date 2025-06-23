using CallOpetatorWebApp.Models;

namespace CallOpetatorWebApp.Services.Kafka
{
    public class KafkaCallsReader : IKafkaCallsReader
    {
        public KafkaCallsReader() { }

        public KafkaOutCall Next()
        {
            return new KafkaOutCall
            {
                ContactFIO = "ТЕСТ",
                Questions = "sdf",
                Welcome = "dddddddddd",
                ContactMobilePhone = "777"
            };
        }
    }
}
