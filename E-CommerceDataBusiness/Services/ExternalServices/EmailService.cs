using E_CommerceDataBusiness.Basic;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Services.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _emailSettings = settings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var MailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Email),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            MailMessage.To.Add(new MailAddress(toEmail));

            using var smtpClint = new SmtpClient(_emailSettings.Host)
            {
                Port = _emailSettings.Port,
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSSL,
                //UseDefaultCredentials = _emailSettings.UseDefaultCredentials,
            };

            await smtpClint.SendMailAsync(MailMessage);

        }
    }
}
