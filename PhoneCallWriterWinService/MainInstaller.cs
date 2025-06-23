using System.ComponentModel;
using System.ServiceProcess;

namespace PhoneCallWriterWinService
{
    [RunInstaller(true)]
    public partial class MainInstaller : System.Configuration.Install.Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public MainInstaller()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "PhoneCallWriterWinService";
            serviceInstaller.Description = 
                "Данная служба дергает каждую минуту SQL запрос, ищет активные обзвоны и звонки в CRM." +
                " Если они найдены, собирает по ним данные и отправляет в Kafka в топик 'CALL_CENTER_ITEM'.";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}