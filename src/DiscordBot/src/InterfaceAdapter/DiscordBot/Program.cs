using Serilog;
using Wsa.Gaas.Werewolf.ChatBot.Application.Common;
using Wsa.Gaas.Werewolf.DiscordBot.DiscordClients;
using Wsa.Gaas.Werewolf.DiscordBot.HostedServices;
using Wsa.Gaas.Werewolf.DiscordBot.Options;

var builder = WebApplication.CreateBuilder(args);

// Set Up Serilog
builder.Host.UseSerilog((cxt, cfg) => cfg.ReadFrom.Configuration(cxt.Configuration));

// Set Up Dependency Injection
var config = builder.Configuration;
builder.Services
    .AddControllers()
    .Services
    .Configure<DiscordBotOptions>(opt => config.Bind(nameof(DiscordBotOptions), opt))
    .Configure<BackendApiEndpointOptions>(opt => config.Bind(nameof(BackendApiEndpointOptions), opt))
    .AddSingleton<IDiscordBotClient, DiscordSocketClientAdapter>()
    .AddSingleton<BackendApi>()
    .AddHostedService<DiscordBotHostedService>()
    ;

// Set Up Application Pipelines
var app = builder.Build();

app.MapControllers();

app.Run();
