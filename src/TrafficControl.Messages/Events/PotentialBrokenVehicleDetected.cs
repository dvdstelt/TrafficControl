namespace TrafficControl.Messages.Events;

public class PotentialBrokenVehicleDetected
{
    public string LicensePlate { get; set; }
    public string Road { get; set; }
    public DateTime Entering { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
}