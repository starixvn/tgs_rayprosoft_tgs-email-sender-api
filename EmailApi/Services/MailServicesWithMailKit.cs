using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;
using TGS.EmailApi.Models;

namespace TGS.EmailApi.Services
{
    public interface IEmailService
    {
        Task SendAsync(SendEmailRequest request);
    }

    public class EmailServiceWithMailKit : IEmailService
    {
        private readonly EmailKitConfiguration _emailConfig;
        private readonly SmtpClient _smtpClient;

        public EmailServiceWithMailKit(IOptions<EmailKitConfiguration> emailConfigAccessor, SmtpClient smtpClient)
        {
            _emailConfig = emailConfigAccessor.Value;
            _smtpClient = smtpClient ?? throw new System.ArgumentNullException(nameof(smtpClient));
        }

        public async Task SendAsync(SendEmailRequest request)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailConfig.FromMail));
            email.To.Add(MailboxAddress.Parse(string.Join(",", request.Receivers)));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Content };

            await _smtpClient.SendAsync(email);
        }
    }
}
