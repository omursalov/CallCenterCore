using CallOpetatorWebApp.Models;
using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using CallOpetatorWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;

namespace CallOpetatorWebApp.Controllers
{
    public class LoginController : Controller
    {
        private ICrmService _crmService;
        private ICacheService _cacheService;

        public LoginController(ICrmService crmService, ICacheService cacheService, IConfiguration configuration)
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
            if (!HttpContext.Session.Keys.Contains("crm-user-session"))
                return View();
            else
                return Redirect("~/OutCall/Process");
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

            if (!string.IsNullOrWhiteSpace(loginModel.UserDomainName)
                && users.FirstOrDefault(x => x["domainname"].ToString().ToLower() == loginModel.UserDomainName.ToLower()) is Entity currCrmUser
                && currCrmUser != null)
            {
                // Сериализуем и пишем инфу о сессии в cache
                HttpContext.Session.SetString("crm-user-session", JsonConvert.SerializeObject(new CrmUserSession
                {
                    Id = Guid.Parse(currCrmUser["systemuserid"].ToString()),
                    DomainName = loginModel.UserDomainName
                }));
                return Redirect("~/OutCall/Process");
            }

            return View();
        }
    }
}