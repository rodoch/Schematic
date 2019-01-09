using System.Threading.Tasks;

namespace Schematic.Core
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}