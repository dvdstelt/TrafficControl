using FineCollection.Messages.Events;
using Shared;

namespace Email.Handlers;

public class EmailPaymentTooLateHandler(EmailService emailService) : IHandleMessages<PaymentTooLate>

{
    public async Task Handle(PaymentTooLate message, IMessageHandlerContext context)
    {
        var roadName = RoadsData.Roads[message.ZoneId].RoadName;
        var subject = $"Urgent: Final Payment Notice";

        await emailService.SendEmailAsync(message.EmailAddress, subject, "PaymentTooLate", new {
            message.FirstName,
            message.LastName,
            message.ViolationInKmh,
            message.LicensePlate,
            message.Fine
        });
    }
}