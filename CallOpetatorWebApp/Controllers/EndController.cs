using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using Microsoft.AspNetCore.Mvc;

namespace CallOpetatorWebApp.Controllers
{
    [ApiController]
    public class EndController : Controller
    {
        private ICrmService _crmService;
        private ICacheService _cacheService;

        public EndController(ICrmService crmService, ICacheService cacheService, IConfiguration configuration)
        {
            _crmService = crmService;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Просто выводим во view, что обзвон закончен.
        /// Удаляем текущую сессию браузера.
        /// Оператор может идти домой.
        /// </summary>
        [HttpGet]
        [Route("End/Index")]
        public IActionResult Index()
        {
            HttpContext.Session.Remove("crm-user-session");
            return View();
        }
    }
}