using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;
using Firmeza.Core.Interfaces;
using System.IO;
using Microsoft.Extensions.Logging; // Añadido

namespace Firmeza.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger; // Añadido

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger) // Añadido logger
        {
            _configuration = configuration;
            _logger = logger; // Asignado
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message, string? attachmentPath = null)
        {
            var emailSettings = _configuration.GetSection("SmtpSettings");
            var smtpServer = emailSettings["Server"];
            var smtpPort = int.Parse(emailSettings["Port"]!);
            var smtpUsername = emailSettings["Username"];
            var smtpPassword = emailSettings["Password"];
            var senderName = emailSettings["SenderName"];
            var senderEmail = emailSettings["SenderEmail"];

            _logger.LogInformation("Intentando enviar correo a {ToEmail} desde {SenderEmail} usando {SmtpServer}:{SmtpPort}", toEmail, senderEmail, smtpServer, smtpPort);

            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(senderName, senderEmail));
                email.To.Add(new MailboxAddress("", toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = message };

                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    _logger.LogInformation("Adjuntando archivo: {AttachmentPath}", attachmentPath);
                    builder.Attachments.Add(attachmentPath);
                }

                email.Body = builder.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    _logger.LogInformation("Conectando a SMTP: {SmtpServer}:{SmtpPort} con StartTls", smtpServer, smtpPort);
                    await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                    _logger.LogInformation("Conectado. Autenticando con usuario: {SmtpUsername}", smtpUsername);
                    await smtp.AuthenticateAsync(smtpUsername, smtpPassword);
                    _logger.LogInformation("Autenticación exitosa. Enviando correo.");
                    await smtp.SendAsync(email);
                    _logger.LogInformation("Correo enviado con éxito. Desconectando.");
                    await smtp.DisconnectAsync(true);
                }
                _logger.LogInformation("Correo enviado a {ToEmail} con éxito.", toEmail);
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                _logger.LogError(ex, "Error de autenticación SMTP al enviar correo. Verifique usuario y contraseña. Mensaje: {Message}", ex.Message);
                throw new ApplicationException("Error de autenticación SMTP. Verifique las credenciales.", ex);
            }
            catch (MailKit.Net.Smtp.SmtpCommandException ex)
            {
                _logger.LogError(ex, "Error de comando SMTP al enviar correo. Código: {ErrorCode}, Mensaje: {Message}", ex.ErrorCode, ex.Message);
                throw new ApplicationException("Error de comando SMTP al enviar correo.", ex);
            }
            catch (MailKit.Net.Smtp.SmtpProtocolException ex)
            {
                _logger.LogError(ex, "Error de protocolo SMTP al enviar correo. Mensaje: {Message}", ex.Message);
                throw new ApplicationException("Error de protocolo SMTP al enviar correo.", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                _logger.LogError(ex, "Error de conexión de socket al enviar correo. Verifique el servidor y puerto SMTP. Mensaje: {Message}", ex.Message);
                throw new ApplicationException("Error de conexión al servidor SMTP. Verifique la configuración de red.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al enviar correo a {ToEmail}. Mensaje: {Message}", toEmail, ex.Message);
                throw new ApplicationException("Error inesperado al enviar correo.", ex);
            }
        }
    }
}
