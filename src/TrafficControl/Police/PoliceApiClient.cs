using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace TrafficControl.Police;

public class PoliceApiClient(HttpClient httpClient, ILogger<PoliceApiClient> logger)
{
    public async Task<IEnumerable<string>> GetWantedPlateHashesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<PlatesResponse>(
                "api/police/wanted-plates", ct);
            return response?.Hashes ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve wanted plate hashes");
            throw;
        }
    }

    public async Task NotifyPolice(string licensePlate, string roadId, DateTime timestamp,
        CancellationToken ct = default)
    {
        var detection = new
        {
            LicensePlate = licensePlate,
            Road = roadId,
            Timestamp = timestamp
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync(
                "api/police/vehicle-detections", detection, ct);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to record detection for plate {LicensePlate} on road {RoadId}",
                detection.LicensePlate, detection.Road);
            throw;
        }
    }

    private record PlatesResponse(DateTime Timestamp, IEnumerable<string> Hashes);
}