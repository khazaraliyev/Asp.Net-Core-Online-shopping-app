using Microsoft.AspNetCore.Identity;
using ShopApp.WebUI.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.ViewModels
{
    public class RoleViewModel
    {
        [Required(ErrorMessage ="Enter role name")]
        public string Name { get; set; }
    }

    public class RoleDetails
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
    }

    public class EditRole
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string[] IdToAdd{ get; set; }
        public string[] IdToDelete { get; set; }
    }
}
