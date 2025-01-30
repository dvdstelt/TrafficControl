namespace FineCollection.Messages.Events;

public class FineRecorded
{
    public int ViolationInKmh { get; set; }
    public string LicensePlate { get; set; }
    public int ZoneId { get; set; }
    public string EmailAddress { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CreditCardObfuscated { get; set; }
    public decimal Fine { get; set; }
}