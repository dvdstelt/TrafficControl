using System.Net.Mail;
using FineCollection.Messages.Events;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Shared;

namespace FineCollection.Handlers;

public class EmailOwnerAboutFineHandler(ILogger<EmailOwnerAboutFineHandler> logger) : IHandleMessages<FineRecorded>
{
    readonly SmtpClient smtpClient = new("localhost", 25);

    public async Task Handle(FineRecorded message, IMessageHandlerContext context)
    {
            var roadName = RoadsData.Roads[message.ZoneId].RoadName;

            var emailBody = $"""
                             Dear {message.FirstName} {message.LastName},

                             Our speed detection system clocked your vehicle ({message.LicensePlate}) traveling at {message.ViolationInKmh} km/h on the {roadName}.
                             This exceeds the speed limit of 100 km/h.

                             As a result, you have incurred a fine of €{message.Fine:N2} on your credit card ending with {message.CreditCardObfuscated}.

                             Please ensure future compliance with speed limits for everyone's safety.

                             Best regards,
                             Centraal Justitieel Incassobureau
                             """;

            var mailMessage = new MailMessage
            {
                From = new MailAddress("noreply@cjib.nl"),
                Subject = $"Speed Violation on {roadName}",
                Body = emailBody
            };
            mailMessage.To.Add(message.EmailAddress);

            await smtpClient.SendMailAsync(mailMessage, context.CancellationToken);
            logger.LogInformation("Fine notification sent for vehicle {VehicleId}. Amount: {Fine} euro", message.LicensePlate, message.Fine);
    }
}