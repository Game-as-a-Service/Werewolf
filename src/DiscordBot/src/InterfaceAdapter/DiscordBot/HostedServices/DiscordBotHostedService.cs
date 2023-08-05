using System.Net.Security;
using Wsa.Gaas.Werewolf.ChatBot.Application.Common;

namespace Wsa.Gaas.Werewolf.DiscordBot.HostedServices;

public class DiscordBotHostedService : BackgroundService
{
    private readonly IDiscordBotClient _discordBotClient;

    public DiscordBotHostedService(IDiscordBotClient discordClient)
    {
        _discordBotClient = discordClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _discordBotClient.StartAsync();
    }
}
