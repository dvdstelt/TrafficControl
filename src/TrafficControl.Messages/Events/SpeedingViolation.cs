namespace TrafficControl.Messages.Events;

public class SpeedingViolation
{
    public int ZoneId { get; set; }
    public string LicensePlate { get; init; }
    public int ViolationInKmh { get; init; }
    public DateTime Timestamp { get; init; }
}