using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Account
{
    public class LoginViewModel
    {
        [Required, MaxLength(256)]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Запомнить на сайте")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}