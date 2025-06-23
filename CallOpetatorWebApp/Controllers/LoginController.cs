using Microsoft.AspNetCore.Mvc;

namespace CallOpetatorWebApp.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}