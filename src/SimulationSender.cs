using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Simulation";

        // Configure the endpoint
        var endpointConfiguration = new EndpointConfiguration("Simulation");
        endpointConfiguration.UseTransport<LearningTransport>();
        endpointConfiguration.SendOnly();

        // Start the endpoint
        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        Console.WriteLine("Simulation started. Press any key to exit ...");
        Console.ReadKey();

        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}