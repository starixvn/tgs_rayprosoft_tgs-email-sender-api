using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TGS.EmailApi.Controllers;
using TGS.EmailApi.Models;
using TGS.EmailApi.Services;

namespace EmailApi.Tests
{
    [TestFixture]
    public class EmailControllerTest
    {
        public EmailControllerTest() 
        { 
        
        }

        [Test]
        public async Task Can_Send_Email_Successfully()
        {
            var loggerMock = new Mock<ILogger<EmailController>>();
            var emailNaanagerMock = new Mock<EmailService>();

            var sendEmailRequest = new SendEmailRequest(new[] { "admin@rayprosoft.com" }, "Subject Test", "Content Tests");

            var controller = new EmailController(loggerMock.Object, emailNaanagerMock.Object);
            var response = await controller.SendEmailAsync(sendEmailRequest);
            Assert.IsTrue(response.IsSuccessful);
        }
    }
}
