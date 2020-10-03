using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.EmailService
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string messagesubject, string messagebody);
    }
}
