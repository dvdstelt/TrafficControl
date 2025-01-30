namespace TrafficControl.Messages.Commands;

public class VehicleExiting
{
    public required int ZoneId { get; init; }
    public required string LicensePlate { get; init; }
    public required DateTime ExitTimestamp { get; init; }
}