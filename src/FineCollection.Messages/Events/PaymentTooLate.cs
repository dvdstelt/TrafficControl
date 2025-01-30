namespace FineCollection.Messages.Events;

public class PaymentTooLate
{
    public string EmailAddress { get; set; }
    public string LicensePlate { get; set; }
    public int ZoneId { get; set; }
    public int ViolationInKmh { get; set; }
    public decimal Fine { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}