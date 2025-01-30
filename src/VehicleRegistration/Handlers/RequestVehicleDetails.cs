using Microsoft.Extensions.Logging;
using VehicleRegistrationMessages.Messages;

namespace VehicleRegistration.Handlers;

public class RequestVehicleDetails(ILogger<RequestVehicleDetails> logger, RandomNamesGenerator randomNamesGenerator) : IHandleMessages<VehicleDetailsRequest>
{
    public async Task Handle(VehicleDetailsRequest message, IMessageHandlerContext context)
    {
        var (brand, model) = randomNamesGenerator.GenerateRandomCar();
        var (firstName, lastName, email, creditCard) = randomNamesGenerator.GenerateRandomUser();

        var response = new VehicleDetailsResponse
        {
            CarBrand = brand,
            CarModel = model,
            LicensePlate = message.LicensePlate,
            OwnedAt = message.OwnedAt,
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = email,
            CreditCardObfuscated = creditCard
        };

        logger.LogInformation("I came up with {Brand}, {Model}, {Firstname}, {Lastname}, {Email}, {Creditcard}", brand, model, firstName, lastName, email, creditCard);

        await context.Reply(response);
    }
}