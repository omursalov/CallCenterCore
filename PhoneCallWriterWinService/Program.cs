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
            ServiceBase.Run(new ServiceBase[]
            {
                new CallKafkaWriter()
            });
        }
    }
}