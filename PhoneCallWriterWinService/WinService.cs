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
using System.Threading.Tasks;

namespace PhoneCallWriterWinService
{
    /// <summary>
    /// Пишет звонки CRM в Kafka, служба работает всегда.
    /// Все выполняется в одном потоке.
    /// </summary>
    public partial class WinService : ServiceBase
    {
        private Task _task;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _token;

        public WinService()
        {
            InitializeComponent();
            _cancelTokenSource = new CancellationTokenSource();
            _token = _cancelTokenSource.Token;
        }

        /// <summary>
        /// Пока идет тестирование, он public.
        /// Потом переведем на private.
        /// UPD - не совсем корректно, оказывается блочить OnStart не надо.
        /// Сделаем таску..
        /// </summary>
        public void OnStart()
        {
            _task = Task.Run(() =>
            {
                var logger = LogManager.GetCurrentClassLogger();

                // В будущем нужен норм SSL сертификат, пока так (для теста)
                ServicePointManager.ServerCertificateValidationCallback =
                   delegate (object sender, X509Certificate certificate, X509Chain chain,
                   SslPolicyErrors sslPolicyErrors) { return true; };

                var crmDbWorker = new CrmDbWorker();

                try
                {
                    crmDbWorker.Ping();
                    logger.Info("Подключение к CRM DB успешно");
                }
                catch (Exception ex)
                {
                    logger.Error($"{nameof(CrmDbWorker)}: {ex}");
                }

                using (var kafkaProducer = new KafkaProducer(ConfigurationManager.AppSettings["TopicName"]))
                {
                    var crmClient = new OnPremiseClient(
                    ConfigurationManager.AppSettings["CrmOrgServiceUrl"],
                    ConfigurationManager.AppSettings["CrmLogin"],
                    ConfigurationManager.AppSettings["CrmPass"]);

                    try
                    {
                        var response = (WhoAmIResponse)crmClient.Execute(new WhoAmIRequest());
                        logger.Info($"WhoAmIResponse.UserId = {response.UserId}");
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"{nameof(OnPremiseClient)}: {ex}");
                    }

                    while (true)
                    {
                        var callClientsEntities = crmDbWorker.GetActiveCallClientsEntities();

                        if (_token.IsCancellationRequested)
                            break;

                        if (callClientsEntities.Count > 0)
                        {
                            logger.Info($"Найдено {callClientsEntities.Count} активных обзвонов");
                            foreach (var callClientsEntity in crmDbWorker.GetActiveCallClientsEntities())
                            {
                                foreach (var phoneCall in callClientsEntity.PhoneCalls)
                                {
                                    kafkaProducer.Execute(MsgConverter.Execute(callClientsEntity, phoneCall));
                                    logger.Info($"Звонок {phoneCall.Id} улетел в Kafka");
                                }

                                crmClient.Execute(new SetStateRequest
                                {
                                    EntityMoniker = new EntityReference("new_calling_contacts_entity", callClientsEntity.Id),
                                    State = new OptionSetValue(0),
                                    Status = new OptionSetValue(100000000)
                                });
                                logger.Info($"Обзвон {callClientsEntity.Id} перешел в statuscode = 'Загружено в Kafka'");

                                if (_token.IsCancellationRequested)
                                    break;
                            }
                        }
                        else
                        {
                            var DELAY_IN_SEC = 30;
                            logger.Info($"Не найдено активных обзвонов, DELAY_IN_SEC = {DELAY_IN_SEC}");
                            Thread.Sleep(DELAY_IN_SEC * 1000);
                        }
                    }
                }
            }, _token);
        }

        /// <summary>
        /// Ожидаем, чтобы _task завершилась
        /// </summary>
        public void Stop()
        {
            var WAIT_CANCEL_IN_SEC = 60;
            _cancelTokenSource.Cancel();
            Thread.Sleep(WAIT_CANCEL_IN_SEC * 1000);
        }

        /// <summary>
        /// Находим в CRM активные обзвоны 
        /// и их активные звонки, а также информацию о контактах.
        /// Пишем информацию в Kafka в топик (JSON).
        /// Обзвоны, когда все их звонки загружены в Kafka,
        /// переходят в состояние "Загружен в Kafka".
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args) => OnStart();

        protected override void OnStop() => Stop();
    }
}