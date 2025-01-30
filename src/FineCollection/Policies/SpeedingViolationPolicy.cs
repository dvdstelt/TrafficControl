using FineCollection.Messages.Events;
using Microsoft.Extensions.Logging;
using Shared;
using TrafficControl.Messages.Events;
using VehicleRegistrationMessages.Messages;

namespace FineCollection.Policies;

public class MyData : ContainSagaData
{
    public string LicensePlate { get; set; } = null!;
    public DateTime TimeStamp { get; set; }
    public int ViolationInKmh { get; set; }
    public int ZoneId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
}

public class SpeedingViolationPolicy(ILogger<SpeedingViolationPolicy> logger) : Saga<MyData>,
    IAmStartedByMessages<SpeedingViolation>,
    IHandleMessages<VehicleDetailsResponse>,
    IHandleTimeouts<SpeedingViolationPolicy.VerifyPayment>
{
    static readonly Random Random = Random.Shared;

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MyData> mapper)
    {
        mapper.MapSaga(s => s.LicensePlate)
            .ToMessage<SpeedingViolation>(msg => msg.LicensePlate);
    }

    public async Task Handle(SpeedingViolation message, IMessageHandlerContext context)
    {
        Data.TimeStamp = message.Timestamp;
        Data.ViolationInKmh = message.ViolationInKmh;
        Data.ZoneId = message.ZoneId;

        var request = new VehicleDetailsRequest()
        {
            LicensePlate = message.LicensePlate,
            OwnedAt = message.Timestamp
        };

        await context.Send(request);
    }

    public async Task Handle(VehicleDetailsResponse message, IMessageHandlerContext context)
    {
        logger.LogInformation("Vehicle {VehiclePlate} - Violation: {kmh} - {FirstName} at {Email} with creditcard {CreditCard}", message.LicensePlate, Data.ViolationInKmh ,message.FirstName, message.EmailAddress, message.CreditCardObfuscated);

        // We might need to send another email if payment doesn't arrive in time.
        Data.FirstName = message.FirstName;
        Data.LastName = message.LastName;
        Data.EmailAddress = message.EmailAddress;

        var @event = new FineRecorded()
        {
            EmailAddress = message.EmailAddress,
            LicensePlate = message.LicensePlate,
            ZoneId = Data.ZoneId,
            ViolationInKmh = Data.ViolationInKmh,
            Fine = CalculateFine(Data.ViolationInKmh),
            FirstName = message.FirstName,
            LastName = message.LastName,
            CreditCardObfuscated = message.CreditCardObfuscated
        };

        await context.Publish(@event);
        await RequestTimeout<VerifyPayment>(context, TimeSpan.FromSeconds(10));
    }

    public async Task Timeout(VerifyPayment state, IMessageHandlerContext context)
    {
        if (Random.NextDouble() < 0.25) // 25% chance payment wasn't done
        {
            var @event = new PaymentTooLate()
            {
                EmailAddress = Data.EmailAddress,
                LicensePlate = Data.LicensePlate,
                ZoneId = Data.ZoneId,
                ViolationInKmh = Data.ViolationInKmh,
                Fine = CalculateFine(Data.ViolationInKmh),
                FirstName = Data.FirstName,
                LastName = Data.LastName,
            };

            await context.Publish(@event);
        }
    }

    static decimal CalculateFine(int actualSpeed)
    {
        var kmOverLimit = actualSpeed - 100;
        return kmOverLimit switch
        {
            <= 5 => 50m,
            <= 10 => 137m,
            <= 15 => 207m,
            <= 20 => 285m,
            <= 25 => 377m,
            <= 30 => 475m,
            <= 35 => 582m,
            _ => 699m
        };
    }

    public class VerifyPayment
    {
    }
}

