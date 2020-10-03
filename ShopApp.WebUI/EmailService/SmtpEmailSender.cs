using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ShopApp.WebUI.EmailService
{
    public class SmtpEmailSender : IEmailSender
    {
        private string host;
        private int port;
        private bool enableSSL;
        private string username;
        private string password;

        public SmtpEmailSender(string host, int port, bool enableSSL, string username, string password)
        {
            this.host = host;
            this.port = port;
            this.enableSSL = enableSSL;
            this.username = username;
            this.password = password;
        }
        public Task SendEmailAsync(string email, string messagesubject, string messagebody)
        {
            var client = new SmtpClient(this.host, this.port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = this.enableSSL
            };
            return client.SendMailAsync(
                new MailMessage(this.username, email, messagesubject, messagebody)
                {
                    IsBodyHtml = true,
                }
             );
        }
    }
}
