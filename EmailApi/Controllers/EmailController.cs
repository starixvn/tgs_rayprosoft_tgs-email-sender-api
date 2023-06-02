using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TGS.EmailApi.Metrics;
using TGS.EmailApi.Models;
using TGS.EmailApi.Services;

namespace TGS.EmailApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private static readonly ActivitySource _emailControllerSource = new($"TGS.EmailApi.Controller.{nameof(EmailController)}");
        private readonly ILogger<EmailController> _logger;
        private readonly IEmailService _emailService;
        private readonly EmailMetrics _metrics;

        public EmailController(ILogger<EmailController> logger, 
            [FromServices] IEmailService emailService,
            [FromServices] EmailMetrics metrics)
        {
            _logger = logger;
            _emailService = emailService;
            _metrics = metrics;
        }

        [HttpPost]
        public async Task<SendEmailResponse> SendEmailAsync([FromBody]SendEmailRequest sendEmailRequest)
        {
            var stringBuilder = new StringBuilder();
            foreach (var header in Request.Headers)
            {
                stringBuilder.Append($"key:{header.Key}, value:{header.Value}\n");
            }
            _logger.LogInformation(stringBuilder.ToString());

            using var activity = _emailControllerSource.StartActivity(nameof(EmailController));
            EmailPackage mailMessage = new EmailPackage(
                   sendEmailRequest.Receivers,
                   sendEmailRequest.Subject,
                   sendEmailRequest.Content);
            try
            {
                _logger.LogInformation($"Receiver:{String.Join(',', sendEmailRequest.Receivers)}");
                _logger.LogInformation($"Content:{sendEmailRequest.Content}");
                await _emailService.SendEmailAsync(mailMessage, _emailControllerSource).ConfigureAwait(false);
                _metrics.AddEmailCount();
                return new SendEmailResponse();
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                        System.Threading.Thread.Sleep(5000);
                        await _emailService.SendEmailAsync(mailMessage, _emailControllerSource).ConfigureAwait(false);
                        return new SendEmailResponse();
                    }
                    else
                    {
                        var message = $"Failed to deliver message to {ex.InnerExceptions[i].FailedRecipient}";
                        _logger.LogError(message);
                        return new SendEmailResponse(false, message);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                var message = $"Exception caught in RetryIfBusy(): {ex}";

                _logger.LogError(message);
                return new SendEmailResponse(false, message);
            }
        }
    }
}
