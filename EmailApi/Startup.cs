using System;
using System.IO;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Mail;

namespace TGS.EmailApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmailApi", Version = "v1" });
            });
            services.Configure<EmailConfiguration>(Configuration.GetSection("EmailSetting"));
            services.AddMail();
            services.AddSingleton<IJsonSerializationProvider, JsonSerializationProvider>();

            services.AddControllers(options => options.Filters.Add<ValidateModelFilter>())
               .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
               .AddFluentValidation(c =>
               {
                   c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                   c.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                   c.LocalizationEnabled = false;
                   c.ImplicitlyValidateChildProperties = true;
               });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS Agent API"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
