using PhoneCallWriterWinService.Kafka;
using System.ServiceProcess;

namespace PhoneCallWriterWinService
{
    /// <summary>
    /// Пишет звонки CRM в Kafka, служба работает всегда
    /// </summary>
    public partial class WinService : ServiceBase
    {
        private readonly KafkaProducer _kafkaProducer;

        public WinService()
        {
            InitializeComponent();
            _kafkaProducer = new KafkaProducer();
        }

        protected override void OnStart(string[] args)
        {
            _kafkaProducer.Write("");
        }

        protected override void OnStop()
        {
            _kafkaProducer.Dispose();
        }
    }
}