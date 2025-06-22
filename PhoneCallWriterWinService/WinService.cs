using Data8.PowerPlatform.Dataverse.Client;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NLog;
using PhoneCallWriterWinService.DB.CRM;
using PhoneCallWriterWinService.Kafka;
using System;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Threading;

namespace PhoneCallWriterWinService
{
    /// <summary>
    /// Пишет звонки CRM в Kafka, служба работает всегда.
    /// Все выполняется в одном потоке.
    /// </summary>
    public partial class WinService : ServiceBase
    {
        private readonly CrmDbWorker _crmDbWorker;
        private readonly KafkaProducer _kafkaProducer;
        private readonly OnPremiseClient _crmClient;

        private const int DELAY_IN_MIN = 1;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public WinService()
        {
            InitializeComponent();

            _crmDbWorker = new CrmDbWorker();

            try
            {
                _crmDbWorker.Ping();
                _logger.Info();
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(CrmDbWorker)}: {ex}");
            }

            _kafkaProducer = new KafkaProducer(ConfigurationManager.AppSettings["TopicName"]);

            // В будущем нужен норм SSL сертификат, пока так (для теста)
            ServicePointManager.ServerCertificateValidationCallback =
               delegate (object sender, X509Certificate certificate, X509Chain chain,
                   SslPolicyErrors sslPolicyErrors) { return true; };

            _crmClient = new OnPremiseClient(
                ConfigurationManager.AppSettings["CrmOrgServiceUrl"],
                ConfigurationManager.AppSettings["CrmLogin"],
                ConfigurationManager.AppSettings["CrmPass"]);

            try
            {
                var response = (WhoAmIResponse)_crmClient.Execute(new WhoAmIRequest());
                _logger.Info($"WhoAmIResponse.UserId = {response.UserId}");
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(OnPremiseClient)}: {ex}");
            }
        }

        /// <summary>
        /// Каждую минуту находим в CRM активные обзвоны 
        /// и их активные звонки, а также информацию о контактах.
        /// Пишем информацию в Kafka в топик (JSON).
        /// Обзвоны, когда все их звонки загружены в Kafka,
        /// переходят в состояние "Загружен в Kafka".
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            while (true)
            {
                foreach (var callClientsEntity in _crmDbWorker.GetActiveCallClientsEntities())
                {
                    foreach (var phoneCall in callClientsEntity.PhoneCalls)
                        _kafkaProducer.Execute(MsgConverter.Execute(callClientsEntity, phoneCall));
                    _crmClient.Execute(new SetStateRequest
                    {
                        EntityMoniker = new EntityReference("new_calling_contacts_entity", callClientsEntity.Id),
                        State = new OptionSetValue(0),
                        Status = new OptionSetValue(100000000)
                    });
                }
                Thread.Sleep(DELAY_IN_MIN * 1000);
            }
        }

        /// <summary>
        /// Вероятно, стоит в будущем ориентироваться на метод OnStart,
        /// ожидать, когда _kafkaProducer ответит, когда он будет не занят..
        /// И, вроде, когда отрабатывает OnStop, дается сколько-то времени на завершение службы..
        /// В общем, тут еще подумать на будущее..
        /// </summary>
        protected override void OnStop() => _kafkaProducer.Dispose();
    }
}