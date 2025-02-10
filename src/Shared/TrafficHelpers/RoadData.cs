namespace Shared;

public record RoadData(int ZoneId, string RoadName, double LengthInKm, double ExitGateHectometer, double SpeedLimit)
{
}

public static class RoadsData
{
    // Public static readonly collection
    public static readonly List<RoadData> Roads =
    [
        new RoadData(0, "A12", 6.0, 26.8, 100),
        new RoadData(1, "A20", 4.0, 39.1, 100),
        new RoadData(2, "A12", 10.4, 28.4, 100)
    ];
}
