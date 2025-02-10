namespace PoliceApi.Messages.Events;

public class PoliceWantedVehicleDetected
{
    public required string LicensePlate { get; set; }
    public required string Road { get; set; }
    public DateTime TimeSeen { get; set; }
}