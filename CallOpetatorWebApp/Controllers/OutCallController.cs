using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using Microsoft.AspNetCore.Mvc;

namespace CallOpetatorWebApp.Controllers
{
    public class OutCallController : Controller
    {
        private ICrmService _crmService;
        private ICacheService _cacheService;

        public OutCallController(ICrmService crmService, ICacheService cacheService)
        {
            _crmService = crmService;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Совершается чтение звонка из Kafka и пошло..
        /// </summary>
        [HttpGet]
        public IActionResult Process()
        {
            return View();
        }
    }
}