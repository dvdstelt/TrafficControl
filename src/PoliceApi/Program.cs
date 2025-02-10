using PoliceApi.Extensions;
using PoliceApi.Services;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<WantedPlatesService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DemoPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var endpointConfiguration = new EndpointConfiguration("PoliceApi").Configure();
endpointConfiguration.SendOnly();
builder.UseNServiceBus(endpointConfiguration);

var app = builder.Build();

app.UseCors("DemoPolicy");

app.MapPoliceEndpoints();

app.Run();