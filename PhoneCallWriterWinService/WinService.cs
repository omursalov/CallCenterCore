using PhoneCallWriterWinService.DB.CRM;
using PhoneCallWriterWinService.Kafka;
using System.ServiceProcess;
using System.Threading;

namespace PhoneCallWriterWinService
{
    /// <summary>
    /// Пишет звонки CRM в Kafka, служба работает всегда
    /// </summary>
    public partial class WinService : ServiceBase
    {
        private readonly CrmDbWorker _crmDbWorker;
        private readonly KafkaProducer _kafkaProducer;
        private const int DELAY_IN_MIN = 1;

        public WinService()
        {
            InitializeComponent();
            _crmDbWorker = new CrmDbWorker();
            _kafkaProducer = new KafkaProducer();
        }

        /// <summary>
        /// Каждую минуту находим в CRM активные обзвоны 
        /// и их активные звонки, а также информацию о контактах.
        /// Пишем информацию в Kafka в топик (JSON).
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            while (true)
            {
                foreach (var callClientsEntity in _crmDbWorker.GetActiveCallClientsEntities())
                {
                    foreach (var phoneCall in callClientsEntity.PhoneCalls)
                        _kafkaProducer.Write(MsgConverter.Execute(callClientsEntity, phoneCall));
                }
                Thread.Sleep(DELAY_IN_MIN * 1000);
            }
        }

        protected override void OnStop() => _kafkaProducer.Dispose();
    }
}