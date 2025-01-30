using Microsoft.Extensions.Configuration;

namespace Shared;

public static class ConfigurationExtensions
{
    public static ConfigurationBuilder Configure(this ConfigurationBuilder configurationBuilder, string[] args)
    {
        configurationBuilder
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        return configurationBuilder;
    }
}