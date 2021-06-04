using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TGS.Core.Infrastructures.Mail;
using TGS.EmailApi.Controllers;

namespace EmailApi.Tests
{
    public class EmailControllerTest
    {
        public EmailControllerTest() 
        { 
        
        }
        public async Task Can_Send_Email_Successfully()
        {
            var loggerMock = new Mock<ILogger<EmailController>>();
            var emailNaanagerMock = new Mock<EmailManager>();

            //var sendEmailRequest = new SendEmailRequest(new[] { "lester.wei@rayprosoft.com" }, "Subject Test", "Content Tests");

            //var controller =  new EmailController(loggerMock.Object, emailNaanagerMock.Object);
            //var response = await controller.SendEmailAsync(sendEmailRequest);
            Assert.IsTrue(true);
        }
    }
}
