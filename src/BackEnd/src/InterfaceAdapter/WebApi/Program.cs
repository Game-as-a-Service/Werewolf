using FastEndpoints;
using FastEndpoints.Swagger;
using Wsa.Gaas.Werewolf.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi();
builder.Services.AddSwaggerDoc();

var app = builder.Build();

// Error Handling
app.UseDefaultExceptionHandler();

// Web API
app.UseFastEndpoints();

// SignalR
app.MapHub<GameEventHub>(WebApiDefaults.SignalrEndpoint);

// Swagger
app.UseOpenApi();
app.UseSwaggerUi3(c => c.ConfigureDefaults());

app.Run();
