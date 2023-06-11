using FastEndpoints;
using FastEndpoints.Swagger;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi();
builder.Services.SwaggerDocument(opt =>
{
    
});

var app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<IRepository>().Initialize();

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
