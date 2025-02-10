using Shared;
using TrafficControl.Messages.Commands;

namespace TrafficControl.Police;

public class SpeedingViolationHandler(WantedPlatesCache plateCache, PoliceApiClient policeClient) : IHandleMessages<VehicleEntering>
{
    public async Task Handle(VehicleEntering message, IMessageHandlerContext context)
    {
        var road = RoadsData.Roads[message.ZoneId];

        if (await plateCache.IsPlateWanted(message.LicensePlate, context.CancellationToken))
        {
            await policeClient.NotifyPolice(message.LicensePlate, road.RoadName, message.EntryTimestamp, context.CancellationToken);
        }
    }
}