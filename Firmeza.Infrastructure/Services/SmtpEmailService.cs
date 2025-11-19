using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;
using Firmeza.Core.Interfaces;

namespace Firmeza.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("SmtpSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = message };

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(emailSettings["Server"], int.Parse(emailSettings["Port"]!), SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
