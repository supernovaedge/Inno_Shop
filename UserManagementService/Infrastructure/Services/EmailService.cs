using System.Net;
using System.Net.Mail;
using UserManagementService.Core.Interfaces;

namespace UserManagementService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;

        public EmailService(string smtpHost, int smtpPort, string fromAddress, string smtpUser, string smtpPass)
        {
            _smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };
            _fromAddress = fromAddress;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var mailMessage = new MailMessage(_fromAddress, to, subject, body)
            {
                IsBodyHtml = true
            };
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
