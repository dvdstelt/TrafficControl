using Microsoft.Extensions.Logging;
using Shared;
using TrafficControl.Messages.Commands;
using TrafficControl.Messages.Events;

namespace TrafficControl.Handlers;

public class VehicleTrackingPolicy(ILogger<VehicleTrackingPolicy> logger) : Saga<VehicleTrackingPolicy.VehicleTrackingData>,
    IAmStartedByMessages<VehicleEntering>,
    IAmStartedByMessages<VehicleExiting>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<VehicleTrackingData> mapper)
    {
        mapper.MapSaga(saga => saga.LicensePlate)
            .ToMessage<VehicleEntering>(msg => msg.LicensePlate)
            .ToMessage<VehicleExiting>(msg => msg.LicensePlate);
    }

    public async Task Handle(VehicleEntering message, IMessageHandlerContext context)
    {
        Data.ZoneId = message.ZoneId;
        Data.EntryTimestamp = message.EntryTimestamp;

        var road = RoadsData.Roads.FirstOrDefault(r => r.ZoneId == Data.ZoneId);
        if (road == null)
        {
            throw new InvalidOperationException($"Zone {Data.ZoneId} not found");
        }

        logger.LogInformation("[{EntryTimestamp:HH:mm:ss}] Vehicle {MessageLicensePlate} entered zone {RoadName}", message.EntryTimestamp, message.LicensePlate, road.RoadName);

        await VerifySpeedLimit(context, road);
    }

    public async Task Handle(VehicleExiting message, IMessageHandlerContext context)
    {
        Data.ZoneId = message.ZoneId;
        Data.ExitTimestamp = message.ExitTimestamp;

        var road = RoadsData.Roads.First(r => r.ZoneId == Data.ZoneId);

        await VerifySpeedLimit(context, road);
    }

    async Task VerifySpeedLimit(IMessageHandlerContext context, RoadData roadData)
    {
        if (Data.EntryTimestamp == default || Data.ExitTimestamp == default)
            return;

        var duration = Data.ExitTimestamp - Data.EntryTimestamp;
        var speed = CalculateSpeed(duration, roadData.LengthInKm);

        logger.LogInformation("[{ExitTimestamp:HH:mm:ss}] Vehicle {MessageLicensePlate} exited zone {RoadName}. Speed: {Speed} km/h", Data.ExitTimestamp, Data.LicensePlate, roadData.RoadName, speed);

        if (speed > roadData.SpeedLimit)
        {
            logger.LogInformation($"  That is too fast, let's notify FineCollection.");

            await context.Publish(new SpeedingViolation
            {
                LicensePlate = Data.LicensePlate,
                ZoneId = roadData.ZoneId,
                ViolationInKmh = (int)speed,
                Timestamp = Data.ExitTimestamp
            });
        }

        MarkAsComplete(); // Removes Saga data
    }

    private static double CalculateSpeed(TimeSpan duration, double roadLength) =>
        Math.Round(roadLength / duration.TotalHours, 1);

    public class VehicleTrackingData : ContainSagaData
    {
        public string LicensePlate { get; set; } = null!;
        public int ZoneId { get; set; }
        public DateTime EntryTimestamp { get; set; }
        public DateTime ExitTimestamp { get; set; }
    }
}