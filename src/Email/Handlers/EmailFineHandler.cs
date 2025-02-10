using FineCollection.Messages.Events;
using Microsoft.Extensions.Logging;
using Shared;

namespace Email.Handlers;

public class EmailFineHandler(EmailService emailService) : IHandleMessages<FineRecorded>
{
    public async Task Handle(FineRecorded message, IMessageHandlerContext context)
    {
        var roadName = RoadsData.Roads[message.ZoneId].RoadName;
        var subject = $"Speed Violation on {roadName}";

        await emailService.SendEmailAsync(message.EmailAddress, subject, "SendFine", new {
            message.FirstName,
            message.LastName,
            ViolationSpeed = message.ViolationInKmh,
            message.LicensePlate,
            message.Fine,
            LastFourDigits = message.CreditCardObfuscated.Substring(message.CreditCardObfuscated.Length - 4)
        });
    }
}