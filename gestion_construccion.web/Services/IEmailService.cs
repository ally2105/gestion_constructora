using System.Threading.Tasks;

namespace gestion_construccion.web.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
