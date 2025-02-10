using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace TrafficControl.Police;

public class WantedPlatesCache(
    PoliceApiClient policeClient,
    ILogger<WantedPlatesCache> logger)
{
    private readonly ILogger<WantedPlatesCache> logger = logger;
    private HashSet<string> wantedHashedPlates = new();
    private readonly SemaphoreSlim syncLock = new(1, 1);
    private DateTime lastUpdate = DateTime.MinValue;
    private readonly TimeSpan refreshInterval = TimeSpan.FromMinutes(15);

    public async Task<bool> IsPlateWanted(string licensePlate, CancellationToken ct = default)
    {
        await RefreshCacheIfNeeded(ct);
        var hashedPlate = HashLicensePlate(licensePlate);
        return wantedHashedPlates.Contains(hashedPlate);
    }

    private async Task RefreshCacheIfNeeded(CancellationToken ct)
    {
        if (DateTime.UtcNow - lastUpdate < refreshInterval) return;

        await syncLock.WaitAsync(ct);
        try
        {
            if (DateTime.UtcNow - lastUpdate < refreshInterval) return;

            var hashes = await policeClient.GetWantedPlateHashesAsync(ct);
            wantedHashedPlates = new HashSet<string>(hashes);
            lastUpdate = DateTime.UtcNow;
        }
        finally
        {
            syncLock.Release();
        }
    }

    private static string HashLicensePlate(string licensePlate)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(licensePlate.ToUpperInvariant());
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}