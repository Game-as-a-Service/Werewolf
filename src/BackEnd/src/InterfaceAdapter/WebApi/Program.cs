using FastEndpoints;
using FastEndpoints.Swagger;
using System.Text.Json.Serialization;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.WebApi;
using Wsa.Gaas.Werewolf.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi();
builder.Services.SwaggerDocument(opt =>
{

});

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
app.UseSwaggerUi3(c => c.ConfigureDefaults());

app.Run();
