using PoliceApi.Services;

namespace PoliceApi.Extensions;

public static class EndpointExtensions
{
    public static RouteGroupBuilder MapPoliceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/police");

        group.MapGet("/wanted-plates", (
            WantedPlatesService platesService,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            logger.LogInformation("Retrieving wanted plate hashes");

            var result = new
            {
                Timestamp = DateTime.UtcNow,
                Hashes = platesService.GetRegisteredPlateHashes()
            };

            return Task.FromResult(Results.Ok(result));
        });

        group.MapGet("/simulation-plates", (
            WantedPlatesService platesService,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            var result = new
            {
                Timestamp = DateTime.UtcNow,
                Plates = platesService.GetRegisteredPlates()
            };

            return Task.FromResult(Results.Ok(result));
        });

        group.MapPost("/vehicle-detections", async (
            VehicleDetection detection,
            WantedPlatesService platesService,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            try
            {
                await platesService.RecordDetection(detection);
                return Task.FromResult(Results.Ok());
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Invalid vehicle detection attempt");
                return Task.FromResult(Results.BadRequest(new { error = ex.Message }));
            }
        });

        return group;
    }
}