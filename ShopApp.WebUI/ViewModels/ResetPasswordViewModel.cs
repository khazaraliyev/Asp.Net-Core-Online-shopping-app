using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage ="Token could be found")]
        public string Token { get; set; }
        [Required(ErrorMessage ="Enter email")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Enter correct email format")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
