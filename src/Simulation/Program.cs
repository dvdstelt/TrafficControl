﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Shared;
using Simulation;
using TrafficControl.Messages.Commands;

const string endpointName = "Simulation";

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
    .UseNServiceBus(_ =>
    {
        var endpointConfiguration = new EndpointConfiguration(endpointName);
        endpointConfiguration.Configure(route =>
        {
            route.RouteToEndpoint(typeof(VehicleEntering).Assembly, "TrafficControl");
        });
        endpointConfiguration.SendOnly();

        return endpointConfiguration;
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<TrafficSimulator>();

        services.AddHttpClient<PoliceApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5225/");
        });
    })
    .Build();

await host.RunAsync();