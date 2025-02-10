using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using TrafficControl.Messages.Commands;

namespace Simulation;

record PlatesResponse(DateTime Timestamp, IEnumerable<string> Plates);

public class TrafficSimulator(
    ILogger<TrafficSimulator> logger,
    IMessageSession messageSession,
    PoliceApiClient policeApiClient) : BackgroundService
{
    static readonly Random Random = Random.Shared;
    IEnumerable<string> wantedLicensePlates = null!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Only PoliceApi knows which license plates TrafficControl needs to watch. But those are hashed.
        // In order to simulate those license plates now and then, we need to fetch them from PoliceApi
        wantedLicensePlates = await policeApiClient
            .RetrieveKnownLicensePlatesForDemoPurposes(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var plate = GetLicensePlate();
            var road = RoadsData.Roads[Random.Next(RoadsData.Roads.Count)];
            var entry = DateTime.UtcNow;

            await messageSession.Send(new VehicleEntering
            {
                LicensePlate = plate,
                ZoneId = road.ZoneId,
                EntryTimestamp = entry
            }, cancellationToken: cancellationToken);

            var duration = CalculateHowLongCarIsInZone(road.LengthInKm);

            var sendOptions = new SendOptions();
            sendOptions.DelayDeliveryWith(TimeSpan.FromSeconds(Random.Next(5, 15)));
            //
            await messageSession.Send(new VehicleExiting
            {
                LicensePlate = plate,
                ZoneId = road.ZoneId,
                ExitTimestamp = entry + duration
            }, sendOptions, cancellationToken: cancellationToken);

            logger.LogInformation(
                "Simulating vehicle {VehiclePlate} entering zone {VehicleRoad}, leaving in: {Duration:mm\\:ss}",
                plate, road.RoadName, duration);

            await Task.Delay(Random.Next(1000, 2500), cancellationToken);
        }
    }

    /// <summary>
    /// Generates a random license plate, but has a 5% chance to return a "wanted" license plate.
    /// </summary>
    /// <returns>A Dutch license plate</returns>
    string GetLicensePlate()
    {
        var plate = LicensePlateGenerator.GenerateLicensePlate();
        if (Random.NextDouble() < 0.05) // 5% chance
        {
            // Plate known by the police is returned
            plate = wantedLicensePlates.ElementAt(Random.Next(wantedLicensePlates.Count() - 1));
            logger.LogInformation("Used licenseplate {Plate} from police database", plate);
        }

        return plate;
    }

    static TimeSpan CalculateHowLongCarIsInZone(double roadLengthInKm)
    {
        const int maxPossibleSpeed = 150;
        const int minPossibleSpeed = 80;
        const int speedLimit = 100;

        // Use multiple random values to skew distribution towards legal speeds
        var baseSpeed = minPossibleSpeed + (speedLimit - minPossibleSpeed) * Math.Pow(Random.NextDouble(), 0.7);

        // Only sometimes generate speeds above the limit
        if (Random.NextDouble() < 0.2) // 20% chance of speeding
        {
            baseSpeed = speedLimit + (maxPossibleSpeed - speedLimit) * Math.Pow(Random.NextDouble(), 2);
        }

        return TimeSpan.FromHours(roadLengthInKm / baseSpeed);
    }
}