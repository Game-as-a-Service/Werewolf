using FastEndpoints;
using System.Text.Json.Serialization;
using Wsa.Gaas.Werewolf.Application;
using Wsa.Gaas.Werewolf.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    // Application
    .AddWerewolfApplication()
    // Infrastructure
    .AddWerewolfInfrastructure()
    // Web API
    .AddWerewolfWebApi(builder.Configuration)
    ;

var app = builder.Build();

// Error Handling
app.UseExceptionHandler(opt => { });

// Web API
app.UseFastEndpoints(c =>
{
    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});

// SignalR
app.MapHub<GameEventHub>(WebApiDefaults.SignalrEndpoint);

// Swagger
app.UseOpenApi();


app.Run();
