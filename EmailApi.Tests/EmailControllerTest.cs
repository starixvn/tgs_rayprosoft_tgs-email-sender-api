using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using TGS.EmailApi.Controllers;
using TGS.EmailApi.Metrics;
using TGS.EmailApi.Models;
using TGS.EmailApi.Services;
using Xunit;
using Xunit.Abstractions;

namespace EmailApi.Tests
{
    public class EmailControllerTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public EmailControllerTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact] // Use Fact instead of Test for xUnit
        public async Task Can_Send_Email_Successfully()
        {
            var loggerMock = new Mock<ILogger<EmailController>>();
            var emailServiceMock = new Mock<EmailService>();
            var telegramSvcMock = new Mock<TelegramService>();
            var metrics = new Mock<EmailMetrics>();

            var sendEmailRequest = new SendEmailRequest(new[] { "admin@rayprosoft.com" }, "Subject Test", "Content Tests");

            var controller = new EmailController(loggerMock.Object, emailServiceMock.Object, metrics.Object, telegramSvcMock.Object);
            var response = await controller.SendEmailAsync(sendEmailRequest);

            // Log output using ITestOutputHelper
            _outputHelper.WriteLine("Email sent successfully with response: " + response.IsSuccessful);

            Assert.True(response.IsSuccessful);
        }
    }
}