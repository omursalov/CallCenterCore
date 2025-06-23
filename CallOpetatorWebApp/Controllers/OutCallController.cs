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
    /// <summary>
    /// Здесь происходит обзвон
    /// </summary>
    [ApiController]
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
        /// Читаем сессию браузера.
        /// Смотрим, есть ли предыдущий звонок, который уже завершен.
        /// Обновляем ему продолжительность общения (в мин), дату завершения звонка.
        /// Завершаем звонок (деактивируем).
        /// Получаем из Kafka новый звонок (Kafka reader is singleton, один на все сессии).
        /// Если не нашли его (в топике нет данных), редиректимся на /End/Index.
        /// Если нашли, тут будет что-то с телефонией (на будущее).
        /// Назначаем звонок на оператора колл центра.
        /// Обновим поле FROM (от кого) на звонке.
        /// Выводим информацию о звонке (он уже идет) во view.
        /// Запускаем через JS таймер на html странице.
        /// </summary>
        [HttpGet]
        [Route("OutCall/Process")]
        public IActionResult Process([FromForm] OutCallModel previous)
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

            var kafkaCall = _kafkaCallsReader.Next();
            if (kafkaCall == null)
                return Redirect("~/End/Index");

            var newCall = new OutCallModel
            {
                OutCall = kafkaCall
            };

            // Где-то здесь будет интеграция с API телефонией, например..

            // Назначаем звонок на оператора колл центра
            crmClient.Execute(new AssignRequest
            {
                Target = new EntityReference("phonecall", newCall.OutCall.PhoneCallId),
                Assignee = new EntityReference("systemuser", crmUserSession.Id)
            });

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