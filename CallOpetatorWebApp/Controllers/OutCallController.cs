using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using CallOpetatorWebApp.Services.Kafka;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Process()
        {
            var call = _kafkaCallsReader.Next();
            return View(call);
        }
    }
}