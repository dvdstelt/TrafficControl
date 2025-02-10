using Email;
using Email.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RazorLight;
using Serilog;
using Shared;

const string endpointName = "Email";

var configuration = new ConfigurationBuilder().Configure(args).Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(context =>
    {
        context.ClearProviders();

        var logConfig = new LoggerConfiguration()
            .Enrich.With<LogEventEnricher>()
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {lvl}] {ShortSourceContext} - {Message:lj}{NewLine}{Exception:NewLine}",
                theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code
            ).CreateLogger();

        context.AddSerilog(logConfig);
    })
    .UseNServiceBus(_ =>
    {
        var endpointConfiguration = new EndpointConfiguration(endpointName);
        endpointConfiguration.Configure();

        return endpointConfiguration;
    })
    .ConfigureServices((_, services) =>
    {
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddTransient<EmailService>();

        services.AddSingleton<RazorLightEngine>(_ =>
        {
            var engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates"))
                .UseMemoryCachingProvider()
                .Build();

            return engine;
        });

    }).Build();

await host.RunAsync();