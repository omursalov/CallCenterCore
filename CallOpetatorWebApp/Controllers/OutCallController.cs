using CallOpetatorWebApp.Models;
using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using CallOpetatorWebApp.Services.Kafka;
using CallOpetatorWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

namespace CallOpetatorWebApp.Controllers
{
    public class OutCallController : Controller
    {
        private ICrmService _crmService;
        private ICacheService _cacheService;
        private IKafkaCallsReader _kafkaCallsReader;

        public OutCallController(ICrmService crmService, ICacheService cacheService,
            IKafkaCallsReader kafkaCallsReader)
        {
            _crmService = crmService;
            _cacheService = cacheService;
            _kafkaCallsReader = kafkaCallsReader;
        }

        /// <summary>
        /// Совершается чтение звонка из Kafka и пошло..
        /// </summary>
        [HttpGet]
        public IActionResult Process(OutCallModel previous)
        {
            var crmClient = _crmService.Client;
            var crmUserSession = JsonConvert.DeserializeObject<CrmUserSession>(
                HttpContext.Session.GetString("crm-user-session"));

            if (previous?.OutCall != null)
            {
                // Ставим продолжительность общения (update)
                crmClient.Update(new Entity
                {
                    Id = previous.OutCall.PhoneCallId,
                    LogicalName = "phonecall",
                    Attributes =
                    {
                        { "actualend", DateTime.UtcNow },
                        { "actualdurationminutes", 10 }
                    }
                });

                // Завершаем предыдущий звонок (деактивируем)
                crmClient.Execute(new SetStateRequest
                {
                    EntityMoniker = new EntityReference("phonecall", previous.OutCall.PhoneCallId),
                    State = new OptionSetValue(1), // Completed
                    Status = new OptionSetValue(4) // Received
                });
            }

            var newCall = new OutCallModel
            {
                OutCall = _kafkaCallsReader.Next()
            };
            // Где-то здесь будет интеграция с API телефонией, например..

            // Назначаем звонок на оператора колл центра
            /*crmClient.Execute(new AssignRequest
            {

            });*/

            var party1 = new[]
            {
                new Entity
                {
                    LogicalName = "activityparty",
                    Attributes =
                    {
                        { "partyid", new EntityReference("systemuser", crmUserSession.Id) }
                    }
                }
            };

            // Обновим поле FROM (от кого)
            crmClient.Update(new Entity
            {
                Id = previous.OutCall.PhoneCallId,
                LogicalName = "phonecall",
                Attributes =
                {
                    { "from", party1 }
                }
            });

            newCall.OperatorId = crmUserSession.Id;
            newCall.OperatorName = crmUserSession.DomainName;

            return View(newCall);
        }
    }
}