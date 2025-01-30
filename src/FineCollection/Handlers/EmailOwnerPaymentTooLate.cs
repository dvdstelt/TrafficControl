using System.Net.Mail;
using FineCollection.Messages.Events;
using Microsoft.Extensions.Logging;
using Shared;

namespace FineCollection.Handlers;

public class EmailOwnerPaymentTooLate(ILogger<EmailOwnerAboutFineHandler> logger) : IHandleMessages<PaymentTooLate>
{
    readonly SmtpClient smtpClient = new("localhost", 25);

    public async Task Handle(PaymentTooLate message, IMessageHandlerContext context)
    {
        var roadName = RoadsData.Roads[message.ZoneId].RoadName;

        var emailBody = $"""
                         Dear {message.FirstName} {message.LastName},

                         Our speed detection system clocked your vehicle ({message.LicensePlate}) traveling at {message.ViolationInKmh} km/h on the {roadName}.
                         This exceeds the speed limit of 100 km/h.
                         
                         You received a notification 10 seconds ago, but the amount of your fine is still unpaid.
                         As a result, you have incurred a fine of €{message.Fine:N2} with an additional €20 for administrative costs.
                         Please pay this fine within 5 seconds. If you don't, we will have to take action on your account.

                         Please ensure future compliance with speed limits for everyone's safety.

                         Best regards,
                         Centraal Justitieel Incassobureau
                         """;

        var mailMessage = new MailMessage
        {
            From = new MailAddress("noreply@cjib.nl"),
            Subject = $"Reminder: Speed Violation on {roadName}",
            Body = emailBody
        };
        mailMessage.To.Add(message.EmailAddress);

        await smtpClient.SendMailAsync(mailMessage, context.CancellationToken);
        logger.LogInformation("Fine notification sent for vehicle {VehicleId}. Amount: {Fine} euro", message.LicensePlate, message.Fine);
    }
}