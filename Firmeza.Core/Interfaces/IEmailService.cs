using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message, string? attachmentPath = null);
    }
}
