using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Configuration;
using System.Linq;
using TGS.EmailApi.Extensions;
using TGS.EmailApi.Metrics;
using TGS.EmailApi.Services;
using TGS.EmailApi.Models;
using Microsoft.Extensions.Options;
using FluentdForward.OpenTelemetry.MessagePack;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Services.AddHealthChecks();

builder.Services.AddMail((config, sp) => {
    sp.GetSection("EmailSetting").Bind(config);
});

// °t¸m Telegram ³]¸m
builder.Services.Configure<TelegramSettings>(builder.Configuration.GetSection("TelegramSetting"));

// ª`¤J TelegramService
builder.Services.AddHttpClient<ITelegramService, TelegramService>((client, sp) =>
{
    var telegramSettings = sp.GetRequiredService<IOptions<TelegramSettings>>().Value;
    return new TelegramService(client, telegramSettings.Token, telegramSettings.ChatId);
});


builder.Services.AddSingleton<IJsonSerializationProvider, JsonSerializationProvider>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP");
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME");
            fluentOptions.UseMessagePack(LogRecordFormatterResolver.GetResolverInstanceWithExtendInfo(
                "MachineName",
                () => Environment.MachineName));
        });
    });

var excludeExtensions = new[] { "html", "ico", "png", "jpg" };

var meters = new OtelMetrics();

builder.Services
    .AddOpenTelemetry()
        .WithTracing(builder => builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddAspNetCoreInstrumentation(options => options.Filter =
            httpContext => !httpContext.Request.Path.HasValue
                || !httpContext.Request.Path.StartsWithSegments("/swagger")
                && !httpContext.Request.Path.StartsWithSegments("/health")
                && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                && !httpContext.Request.Path.StartsWithSegments("/metrics")
                && !excludeExtensions.Any(
                    ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProfileViewExporter()
        .AddOtlpExporter())
    .WithMetrics(meterBuiler => meterBuiler
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
            .AddMeter("Tgs.EmailBucket")
            .AddMeter(meters.MetricName)
            .AddInstrumentation<EmailMetrics>()
            .AddInstrumentation<OtelMetrics>()
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
             .AddView(
                instrumentName: "orders-price",
                new ExplicitBucketHistogramConfiguration { Boundaries = new double[] { 15, 30, 45, 60, 75 } })
            .AddView(
                instrumentName: "orders-number-of-books",
                new ExplicitBucketHistogramConfiguration { Boundaries = new double[] { 1, 2, 5 } })
            .AddOtlpExporter())
    .Services
    .AddProfileViewer()
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

var app = builder.Build();
app.MapControllers();
app.UseSwaggerUI();
app.MapSwagger();
app.MapProfileViewer();
app.MapHealthChecks("/healthz");
app.Run();