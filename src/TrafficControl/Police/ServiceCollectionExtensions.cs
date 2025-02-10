using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace TrafficControl.Police;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPoliceIntegration(this IServiceCollection services, string baseUrl)
    {
        // These services need to be registered _before_ registering the HttpClient
        services.AddSingleton<PoliceApiClient>();
        services.AddSingleton<WantedPlatesCache>();

        services.AddHttpClient<PoliceApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });
        return services;
    }
}