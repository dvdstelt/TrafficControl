using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Shared;
using VehicleRegistrationMessages.Messages;

const string endpointName = "FineCollection";

_ = new ConfigurationBuilder().Configure(args).Build();

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
    .UseNServiceBus(context =>
    {
        var endpointConfiguration = new EndpointConfiguration(endpointName);
        endpointConfiguration.Configure(r =>
        {
            r.RouteToEndpoint(typeof(VehicleDetailsRequest).Assembly, "VehicleRegistration");
        });

        return endpointConfiguration;
    }).Build();

host.Run();