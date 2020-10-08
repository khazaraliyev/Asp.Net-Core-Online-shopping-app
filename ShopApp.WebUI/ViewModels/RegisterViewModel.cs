using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Firstname is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage ="Lastname is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage ="Username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage ="Enter password again")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password does not match")]
        public string ReenterPassword { get; set; }
        [Required(ErrorMessage ="Email is requried")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Enter correct email format")]
        public string Email { get; set; }

    }
}
