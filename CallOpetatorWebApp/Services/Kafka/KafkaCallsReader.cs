using CallOpetatorWebApp.ViewModels;

namespace CallOpetatorWebApp.Services.Kafka
{
    public class KafkaCallsReader : IKafkaCallsReader
    {
        public KafkaCallsReader() { }

        public OutCall Next()
        {
            return new OutCall
            {
                ContactFIO = "ТЕСТ",
                Questions = "sdf",
                Welcome = "dddddddddd",
                ContactMobilePhone = "777"
            };
        }
    }
}
