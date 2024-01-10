using Microsoft.EntityFrameworkCore;
using Prometheus;
using TrampolineCenterAPI.Data;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sentry;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        // Add cfg
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // OPTL
        builder.Services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .ConfigureResource(resource => resource.AddService("TrampolineCenterAPI"))
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter()
                .AddJaegerExporter(opts => {
                    opts.AgentHost = "host.docker.internal";
                    opts.AgentPort = 6831;
                })
                .AddSource("Tracing.NET")
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: "Tracing.NET")
                )
            );

        // Add logging 
        builder.Logging.ClearProviders();
        var logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
        builder.Logging.AddSerilog(logger);

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ClientsDb"));

        // Добавление поддержки CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
                builder
                    .SetIsOriginAllowed(_ => true) // Разрешить запросы с любого источника
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        // Sentry
        builder.WebHost.UseSentry(o =>
        {
            o.Dsn = "https://bff1a4cd5cec83af6f73d1813f96bf2d@o4506507825053696.ingest.sentry.io/4506507831345152";
            // When configuring for the first time, to see what the SDK is doing:
            o.Debug = true;
            // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
            // We recommend adjusting this value in production.
            o.TracesSampleRate = 1.0;
        });

        var app = builder.Build();

        // if (app.Environment.IsDevelopment())
        // {
        //     app.UseSwagger();
        //     app.UseSwaggerUI();
        // }


        app.UseSwagger();
        app.UseSwaggerUI();

        // app.UseHttpsRedirection();

        app.UseRouting();
        // METRICS
        app.UseHttpMetrics();

        // Добавление middleware для обработки CORS перед middleware для авторизации
        app.UseCors();

        app.UseAuthorization();

        app.MapControllers();

        // METRICS
        app.MapMetrics();

        //Sentry
        app.UseSentryTracing();

        app.Run();

        SentrySdk.CaptureMessage("Hello Sentry");
    }
}