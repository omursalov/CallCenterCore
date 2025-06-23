using System.ComponentModel.DataAnnotations;

namespace CallOpetatorWebApp.ViewModels
{
    public class LoginModel
    {
        [Display(Name = "Доменное имя пользователя")]
        public string? UserDomainName { get; set; }
    }
}