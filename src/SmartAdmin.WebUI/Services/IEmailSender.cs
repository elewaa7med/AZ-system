using System.Net.Mail;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        void SendEmailHtmlBodyAsync(string email, string subject, string message);
        void SendEmailWithAttachments(string email, string subject, Attachment attachment);

    }
}
