using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using Microsoft.AspNetCore.Mvc;

namespace CallOpetatorWebApp.Controllers
{
    public class EndController : Controller
    {
        private ICrmService _crmService;
        private ICacheService _cacheService;

        public EndController(ICrmService crmService, ICacheService cacheService, IConfiguration configuration)
        {
            _crmService = crmService;
            _cacheService = cacheService;
        }

        [HttpGet]
        public IActionResult Index() => View();
    }
}