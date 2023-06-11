using FastEndpoints;
using FastEndpoints.ClientGen;
using FastEndpoints.Swagger;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi();
builder.Services.AddSwaggerDoc(s =>
{
    s.DocumentName = "v1";
    s.GenerateCSharpClient(
        settings: s => s.ClassName = "ApiClient",
        destination: "./ApiClient.cs"
    );
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

await app.GenerateClientsAndExitAsync(
    documentName: "v1", //must match doc name above
    destinationPath: builder.Environment.WebRootPath,
    csSettings: c => c.ClassName = "ApiClient",
    tsSettings: null);

app.Run();
