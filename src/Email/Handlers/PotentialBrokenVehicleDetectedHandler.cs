using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Logging;
using RazorLight.Compilation;
using TrafficControl.Messages.Events;

namespace Email.Handlers;

public class PotentialBrokenVehicleDetectedHandler(EmailService emailService) :
    IHandleMessages<PotentialBrokenVehicleDetected>
{
    public async Task Handle(PotentialBrokenVehicleDetected message, IMessageHandlerContext context)
    {
        var subject = $"Potential vehicle breakdown on {message.Road}";
        await emailService.SendEmailAsync("warning@anwb.nl", subject, "PotentialBrokenVehicle", new
        {
            message.LicensePlate,
            message.Brand,
            message.Model,
            RoadName = message.Road,
            message.Entering
        });
    }
}