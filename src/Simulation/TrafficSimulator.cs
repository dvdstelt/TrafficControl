using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using TrafficControl.Messages.Commands;

namespace Simulation;

public class TrafficSimulator(ILogger<TrafficSimulator> logger, IMessageSession messageSession) : BackgroundService
{
    private static readonly Random Random = Random.Shared;
    private readonly List<(string plate, RoadData road, DateTime entry)> activeVehicles = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var plate = GenerateLicensePlate();
            var road = RoadsData.Roads[Random.Next(RoadsData.Roads.Count)];
            var entry = DateTime.UtcNow;

            activeVehicles.Add((plate, road, entry));
            logger.LogInformation("Vehicle {Plate} entering traffic control on {RoadRoadName}. Active vehicles: {ActiveVehiclesCount}", plate, road.RoadName, activeVehicles.Count);

            await messageSession.Send(new VehicleEntering
            {
                LicensePlate = plate,
                ZoneId = road.ZoneId,
                EntryTimestamp = entry
            }, cancellationToken: stoppingToken);

            if (activeVehicles.Count != 0 && Random.Shared.NextDouble() < 0.4)
            {
                var howManyToRemain = (int)(activeVehicles.Count * 0.4);
                var howManyWeStillCanRemove = activeVehicles.Count;
                while (howManyWeStillCanRemove >
                       howManyToRemain) // This assures we only register 75% of the vehicles leaving
                {
                    var vehicleIndex = Random.Next(howManyWeStillCanRemove--);
                    var vehicle = activeVehicles[vehicleIndex];

                    var duration = CalculateHowLongCarIsInZone(vehicle.road.LengthInKm);

                    await messageSession.Send(new VehicleExiting
                    {
                        LicensePlate = vehicle.plate,
                        ZoneId = vehicle.road.ZoneId,
                        ExitTimestamp = vehicle.entry + duration
                    }, cancellationToken: stoppingToken);

                    activeVehicles.RemoveAt(vehicleIndex);
                    logger.LogInformation("Vehicle {VehiclePlate} leaving zone {VehicleRoad}. Duration: {Duration:mm\\:ss}. Active vehicles: {ActiveVehiclesCount}", vehicle.plate, vehicle.road.RoadName, duration, activeVehicles.Count);
                }
            }

            await Task.Delay(Random.Next(1000, 2500), stoppingToken);
        }
    }

    static TimeSpan CalculateHowLongCarIsInZone(double roadLengthInKm)
    {
        const int maxPossibleSpeed = 150;
        const int minPossibleSpeed = 80;
        const int speedLimit = 100;

        // Use multiple random values to skew distribution towards legal speeds
        double baseSpeed = minPossibleSpeed + (speedLimit - minPossibleSpeed) * Math.Pow(Random.NextDouble(), 0.7);

        // Only sometimes generate speeds above the limit
        if (Random.NextDouble() < 0.2) // 20% chance of speeding
        {
            baseSpeed = speedLimit + (maxPossibleSpeed - speedLimit) * Math.Pow(Random.NextDouble(), 2);
        }

        return TimeSpan.FromHours(roadLengthInKm / baseSpeed);
    }

    private static string GenerateLicensePlate()
    {
        var format = Random.Next(4);
        return format switch
        {
            0 => $"{Random.Next(100, 999)}-{RandomLetters(2)}-{Random.Next(10)}",
            1 => $"{Random.Next(1, 10)}-{RandomLetters(2)}-{Random.Next(100, 999)}",
            2 => $"{RandomLetters(1)}-{Random.Next(10, 100)}-{RandomLetters(3)}",
            _ => $"{RandomLetters(3)}-{Random.Next(10, 100)}-{RandomLetters(1)}"
        };
    }

    private static string RandomLetters(int count)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return new string(Enumerable.Repeat(chars, count)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}