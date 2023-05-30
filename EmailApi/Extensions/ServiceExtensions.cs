using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using TGS.EmailApi.Models;
using TGS.EmailApi.Services;

namespace TGS.EmailApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMailKit(
            this IServiceCollection services,
            IConfiguration configuration,
            string configSectionName = "EmailKitSetting")
        {
            return services.AddMailKit((emailConfig, sp) =>
            {
                configuration.GetSection(configSectionName).Bind(emailConfig);
            });
        }

        public static IServiceCollection AddMailKit(
            this IServiceCollection services,
            Action<EmailKitConfiguration, IConfiguration> configure = null)
        {
            if (configure == null)
                services.AddOptions<EmailKitConfiguration>();
            else
                services
                    .AddOptions<EmailKitConfiguration>()
                    .Configure(configure);

            services
                .AddTransient<IEmailService, EmailServiceWithMailKit>()
                .AddTransient(sp =>
                {
                    var emailConfig = sp.GetRequiredService<IOptions<EmailKitConfiguration>>().Value;
                    var smtp = new SmtpClient();
                    smtp.Connect(emailConfig.SmtpServer, emailConfig.SmtpPort, SecureSocketOptions.StartTls);
                    smtp.Authenticate(emailConfig.SmtpUsername, emailConfig.SmtpPassword);
                    return smtp;
                });

            return services;
        }
    }
}
