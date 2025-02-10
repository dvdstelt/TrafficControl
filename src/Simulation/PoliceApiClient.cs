using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Simulation;

public class PoliceApiClient(ILogger<PoliceApiClient> logger, HttpClient client)
{
    public async Task<IEnumerable<string>> RetrieveKnownLicensePlatesForDemoPurposes(CancellationToken cancellationToken)
    {
        var response = await client.GetFromJsonAsync<PlatesResponse>("/api/police/simulation-plates", cancellationToken);

        logger.LogDebug("Successfully retrieved wanted license plates for demo purposes");

        return response?.Plates?.ToList() ?? [];
    }
}