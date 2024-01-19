global using Wsa.Gaas.Werewolf.Application.UseCases;
using FastEndpoints;
using System.Text.Json.Serialization;
using Wsa.Gaas.Werewolf.Application;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddWebApi(builder.Configuration)

    // Application
    .AddWerewolfApplication()

    // Infrastructure
    .AddEntityFrameworkCoreRepository()
    ;

var app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<IRepository>().Initialize();

// Error Handling
app.UseJsonExceptionHandler();

// Web API
app.UseFastEndpoints(c =>
{
    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});

// SignalR
app.MapHub<GameEventHub>(WebApiDefaults.SignalrEndpoint);

// Swagger
app.UseOpenApi();
//app.UseSwaggerUi3(c => c.ConfigureDefaults());

app.Run();
