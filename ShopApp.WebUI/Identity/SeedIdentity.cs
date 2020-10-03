using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Identity
{
    public class SeedIdentity
    {
        public static async Task Seed(UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> rolemanager,IConfiguration configuration)
        {
            var username = configuration["Data:AdminUser:username"];
            var email = configuration["Data:AdminUser:email"];
            var password = configuration["Data:AdminUser:password"];
            var role = configuration["Data:AdminUser:role"];

            if (await usermanager.FindByNameAsync(username)==null)
            {
                await rolemanager.CreateAsync(new IdentityRole(role));
                var user = new ApplicationUser()
                {
                    UserName = username,
                    Email = email,
                    FirstName = "admin",
                    LastName = "admin",
                    EmailConfirmed = true
                };

                var result = await usermanager.CreateAsync(user,password);

                if (result.Succeeded)
                {
                    await usermanager.AddToRoleAsync(user, role);
                }

            }
        }            
    }
}
