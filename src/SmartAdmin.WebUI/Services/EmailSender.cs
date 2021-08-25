using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient smtpClient = new SmtpClient
            {
                Host = "mail5006.smarterasp.net",
                Port = 8889,
                EnableSsl = false,
                Credentials = new NetworkCredential("noreply@az-ltd.com", "P@ssw0rd")
            };
            using (MailMessage mymessage = new MailMessage("noreply@az-ltd.com", email)
            {
                Subject = subject,
                Body = message
            })
            {
                mymessage.From = new MailAddress("noreply@az-ltd.com", "Password Reset");
                await smtpClient.SendMailAsync(mymessage);
            }
        }

        public void SendEmailHtmlBodyAsync(string email, string subject, string message)
        {
            SmtpClient smtpClient = new SmtpClient
            {
                Host = "mail5006.smarterasp.net",
                Port = 8889,
                EnableSsl = false,
                Credentials = new NetworkCredential("noreply@az-ltd.com", "P@ssw0rd")
            };
            using (MailMessage mymessage = new MailMessage("noreply@az-ltd.com", email)
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true,

            })
            {
                mymessage.From = new MailAddress("noreply@az-ltd.com", subject);
                smtpClient.Send(mymessage);
            }
        }

        public void SendEmailWithAttachments(string email, string subject, Attachment attachment)
        {
            SmtpClient smtpClient = new SmtpClient
            {
                Host = "mail5006.smarterasp.net",
                Port = 8889,
                EnableSsl = false,
                Credentials = new NetworkCredential("noreply@az-ltd.com", "P@ssw0rd")
            };
            using (MailMessage mymessage = new MailMessage("noreply@az-ltd.com", email)
            {
                Subject = subject
            })
            {
                mymessage.From = new MailAddress("noreply@az-ltd.com", subject);
                mymessage.Attachments.Add(attachment);
                smtpClient.Send(mymessage);
            }
        }
    }
}
