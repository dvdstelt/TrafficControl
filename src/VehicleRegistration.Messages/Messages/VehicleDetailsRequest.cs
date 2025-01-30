namespace VehicleRegistrationMessages.Messages;

public class VehicleDetailsRequest
{
    public required string LicensePlate { get; set; }
    public required DateTime OwnedAt { get; set; }
}