using Wsa.Gaas.Werewolf.ChatBot.Application.Common;

namespace Wsa.Gaas.Werewolf.DiscordBot.HostedServices;

public class DiscordBotHostedService : BackgroundService
{
    private readonly IDiscordBotClient _chatBotClient;

    public DiscordBotHostedService(IDiscordBotClient discordClient)
    {
        _chatBotClient = discordClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _chatBotClient.StartAsync();
    }
}
