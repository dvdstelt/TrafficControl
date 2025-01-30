namespace TrafficControl.Messages.Commands
{
    public class VehicleEntering
    {
        public required int ZoneId { get; init; }
        public required string LicensePlate { get; init; }
        public required DateTime EntryTimestamp { get; init; }
    }

}