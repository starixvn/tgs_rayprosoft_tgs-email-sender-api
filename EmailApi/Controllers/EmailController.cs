using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.Core.Infrastructures.Mail;

namespace TGS.EmailApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IEmailManager  _emailManager;

        public EmailController(ILogger<EmailController> logger, IEmailManager emailManager)
        {
            _logger = logger;
            _emailManager = emailManager;
        }

        [HttpPost]
        public async Task<SendEmailResponse> SendEmailAsync([FromBody]SendEmailRequest sendEmailRequest)
        {
            EmailPackage mailMessage = new EmailPackage(
                   sendEmailRequest.Receivers,
                   sendEmailRequest.Subject,
                   sendEmailRequest.Content);

            try
            {
               
                await _emailManager.SendEmailAsync(mailMessage).ConfigureAwait(false);

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
                        await _emailManager.SendEmailAsync(mailMessage).ConfigureAwait(false);
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
