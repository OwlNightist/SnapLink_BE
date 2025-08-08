using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _cfg;
        public SmtpEmailSender(IConfiguration cfg) { _cfg = cfg; }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var host = _cfg["Smtp:Host"];
            var port = int.Parse(_cfg["Smtp:Port"] ?? "587");
            var user = _cfg["Smtp:User"];
            var pass = _cfg["Smtp:Pass"];
            var from = _cfg["Smtp:From"];

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(user, pass)
            };
            var mail = new MailMessage(from!, toEmail, subject, htmlBody) { IsBodyHtml = true };
            await client.SendMailAsync(mail);
        }
    }
}
