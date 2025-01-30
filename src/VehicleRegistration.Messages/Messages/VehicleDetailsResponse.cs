namespace VehicleRegistrationMessages.Messages;

public class VehicleDetailsResponse
{
    public required string LicensePlate { get; set; }
    public required DateTime OwnedAt { get; set; }

    public required string CarBrand { get; set; }
    public required string CarModel { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string EmailAddress { get; set; }
    public required string CreditCardObfuscated { get; set; }
}