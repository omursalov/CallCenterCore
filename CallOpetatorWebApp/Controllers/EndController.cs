using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using CallOpetatorWebApp.Services.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace CallOpetatorWebApp.Controllers
{
    [ApiController]
    public class EndController : BaseController
    {
        public EndController(ICrmService crmService, 
            ICacheService cacheService, IKafkaCallsReader kafkaCallsReader)
            : base(crmService, cacheService, kafkaCallsReader) { }

        /// <summary>
        /// Просто выводим во view, что обзвон закончен.
        /// Удаляем текущую сессию браузера.
        /// Оператор может идти домой.
        /// </summary>
        [HttpGet]
        [Route("End/Index")]
        public IActionResult Index()
            => WrapperExecute(() =>
            {
                HttpContext.Session.Remove("crm-user-session");
                return View();
            });
    }
}