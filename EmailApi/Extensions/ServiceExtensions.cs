using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TGS.EmailApi.Models;
using TGS.EmailApi.Services;

namespace TGS.EmailApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMail(
            this IServiceCollection services,
            IConfiguration configuration,
            string configSectionName = "EmailSetting")
        {
            return services.AddMail((emailConfig, sp) =>
            {
                configuration.GetSection(configSectionName).Bind(emailConfig);
            });
        }

        public static IServiceCollection AddMail(
            this IServiceCollection services,
            Action<EmailConfiguration, IConfiguration> configure = null)
        {
            if (configure == null)
                services.AddOptions<EmailConfiguration>();
            else
                services
                    .AddOptions<EmailConfiguration>()
                    .Configure(configure);

            services
                .AddTransient<IEmailService, EmailService>()
                .AddTransient(sp =>
                {
                    var emailConfig = sp.GetRequiredService<IOptions<EmailConfiguration>>().Value;

                    return new SmtpClient
                    {
                        Host = emailConfig.SmtpServer,
                        Port = emailConfig.Port,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(
                            emailConfig.FromMail,
                            emailConfig.Password
                        ),
                        EnableSsl = true
                    };
                });

            return services;
        }
    }
}