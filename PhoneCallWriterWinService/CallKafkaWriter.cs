using System.ServiceProcess;

namespace PhoneCallWriterWinService
{
    public partial class CallKafkaWriter : ServiceBase
    {
        public CallKafkaWriter()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}