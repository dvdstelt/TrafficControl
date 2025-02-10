using System.Security.Cryptography;
using System.Text;
using PoliceApi.Messages.Events;
using Shared;

namespace PoliceApi.Services;

public record VehicleDetection(string LicensePlate, string Road, DateTime Timestamp);

public class WantedPlatesService
{
    const int MaxPlates = 100;
    readonly HashSet<string> registeredHashedPlates;
    readonly string[] registeredPlates = new string[MaxPlates];
    readonly ILogger<WantedPlatesService> logger;
    readonly IMessageSession messageSession;

    public WantedPlatesService(ILogger<WantedPlatesService> logger, IMessageSession messageSession)
    {
        this.logger = logger;
        this.messageSession = messageSession;
        this.registeredHashedPlates = [];
        InitializeRegisteredPlates();
    }

    private void InitializeRegisteredPlates()
    {
        for (int i = 0; i < MaxPlates; i++)
        {
            var plate = LicensePlateGenerator.GenerateLicensePlate();
            var hashedPlate = HashLicensePlate(plate);
            registeredPlates[i] = plate;
            registeredHashedPlates.Add(hashedPlate);
            logger.LogInformation("Added registered plate hash: {Hash} (from {Plate})", hashedPlate, plate);
        }
    }

    public async Task RecordDetection(VehicleDetection detection)
    {
        var hashedPlate = HashLicensePlate(detection.LicensePlate);

        if (!registeredHashedPlates.Contains(hashedPlate))
        {
            logger.LogInformation(
                "Unregistered vehicle notification. License plate: {LicensePlate}, Road: {RoadId}, Time: {Timestamp}",
                detection.LicensePlate, detection.Road, detection.Timestamp);
            return;
        }

        logger.LogInformation(
            "Recorded detection of registered vehicle. License plate: {LicensePlate}, Road: {RoadId}, Time: {Timestamp}",
            detection.LicensePlate, detection.Road, detection.Timestamp);

        var @event = new PoliceWantedVehicleDetected()
        {
            LicensePlate = detection.LicensePlate,
            Road = detection.Road,
            TimeSeen = detection.Timestamp
        };

        await messageSession.Publish(@event);
    }

    private static string HashLicensePlate(string licensePlate)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(licensePlate.ToUpperInvariant());
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    public IEnumerable<string> GetRegisteredPlateHashes() => registeredHashedPlates;
    public IEnumerable<string> GetRegisteredPlates() => registeredPlates;
}