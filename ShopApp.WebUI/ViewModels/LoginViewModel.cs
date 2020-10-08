using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email address is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password is required")]
        [DataType(DataType.Password,ErrorMessage ="Enter correct email format")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
