using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using CallOpetatorWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CallOpetatorWebApp.Controllers
{
    public class LoginController : Controller
    {
        private ICrmService _crmService;
        private ICacheService _cacheService;

        public LoginController(ICrmService crmService, ICacheService cacheService)
        {
            _crmService = crmService;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Начальная страница при запуске сервиса
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Пользователь авторизуется (вводит доменное имя учетки CRM).
        /// Да, пока не использую CrmContext, Set'ы, early bound..
        /// Это потом можно присобачить.
        /// </summary>
        [HttpPost]
        public IActionResult Index(LoginModel loginModel)
        {
            var users = _cacheService.Execute<IList<Entity>>("get_crm_users", () =>
                _crmService.Client.RetrieveMultiple(new QueryExpression
                {
                    EntityName = "systemuser",
                    ColumnSet = new ColumnSet("systemuserid", "domainname"),
                    NoLock = true
                }).Entities.ToList());

            return View("УСПЕХ");
        }
    }
}