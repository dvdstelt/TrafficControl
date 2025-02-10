using PoliceApi.Messages.Events;

namespace Email.Handlers;

public class EmailPoliceAboutWantedVehicle(EmailService emailService) : IHandleMessages<PoliceWantedVehicleDetected>
{
    public async Task Handle(PoliceWantedVehicleDetected message, IMessageHandlerContext context)
    {
        var subject = $"PRIORITY ALERT: Vehicle of interest Detected";

        await emailService.SendEmailAsync("info@politie.nl", subject, "PoliceWantedVehicleDetected", new {
            message.LicensePlate,
            message.Road,
            message.TimeSeen,
        });
    }
}