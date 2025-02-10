using Shared;
using TrafficControl.Messages.Commands;
using TrafficControl.Messages.Events;
using VehicleRegistrationMessages.Messages;

namespace TrafficControl.Policies;

public class TrackPotentialBreakDownPolicy : Saga<TrackPotentialBreakDownPolicy.BreakDownData>,
                IAmStartedByMessages<VehicleEntering>,
                IAmStartedByMessages<VehicleExiting>,
                IHandleMessages<VehicleDetailsResponse>,
                IHandleTimeouts<TrackPotentialBreakDownPolicy.VehiclePotentiallyBroken>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BreakDownData> mapper)
    {
        mapper.MapSaga(saga => saga.LicensePlate)
            .ToMessage<VehicleExiting>(msg => msg.LicensePlate)
            .ToMessage<VehicleEntering>(msg => msg.LicensePlate);
    }

    public async Task Handle(VehicleEntering message, IMessageHandlerContext context)
    {
        Data.EntryTimestamp = message.EntryTimestamp;
        Data.ZoneId = message.ZoneId;

        await RequestTimeout<VehiclePotentiallyBroken>(context, TimeSpan.FromSeconds(12));
    }

    public Task Handle(VehicleExiting message, IMessageHandlerContext context)
    {
        // We mark as complete, which deletes the saga instance data
        // A timeout will still arrive, but be ignored
        // The message VehicleDetailsResponse could come in, but be ignored as well
        // That is okay, because this code will only be executed if the car leaves the area
        MarkAsComplete();

        return Task.CompletedTask;
    }

    public async Task Timeout(VehiclePotentiallyBroken state, IMessageHandlerContext context)
    {
        var request = new VehicleDetailsRequest()
        {
            LicensePlate = Data.LicensePlate,
            OwnedAt = Data.EntryTimestamp
        };

        await context.Send(request);
    }

    public async Task Handle(VehicleDetailsResponse message, IMessageHandlerContext context)
    {
        var @event = new PotentialBrokenVehicleDetected()
        {
            LicensePlate = Data.LicensePlate,
            Road = RoadsData.Roads[Data.ZoneId].RoadName,
            Entering = Data.EntryTimestamp,
            Brand = message.CarBrand,
            Model = message.CarModel
        };
        await context.Publish(@event);
    }

    public class BreakDownData : ContainSagaData
    {
        public string LicensePlate { get; set; } = null!;
        public int ZoneId { get; set; }
        public DateTime EntryTimestamp { get; set; }
    }

    public class VehiclePotentiallyBroken
    {
    }
}