using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace TrafficControl.Police;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPoliceIntegration(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient<PoliceApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddSingleton<WantedPlatesCache>();

        return services;
    }
}