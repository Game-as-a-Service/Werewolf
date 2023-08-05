using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Wsa.Gaas.Werewolf.DiscordBot.DiscordClients
{
    public class AppSettings
    {
        public ulong KogCommandChannelId { get; internal set; }
        public ulong? LogChannelId { get; internal set; }
    }

    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;
        private readonly ILogger<InteractionHandler> _logger;
        private readonly AppSettings _settings;

        public InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services, ILogger<InteractionHandler> logger, IOptions<AppSettings> settings)
        {
            _client = client;
            _commands = commands;
            _services = services;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteractionAsync;
            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ContextCommandExecuted += ContextCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        private static Task ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3) => Task.CompletedTask;

        private static Task ContextCommandExecuted(ContextCommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3) => Task.CompletedTask;

        private static async Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3)
        {
            if (arg3 is { IsSuccess: false, Error: InteractionCommandError.UnmetPrecondition })
                await arg2.Interaction.RespondAsync(arg3.ErrorReason, ephemeral: true);
        }

        private async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(_client, interaction);
                
                var result = await _commands.ExecuteCommandAsync(context, _services);

                if (!result.IsSuccess)
                {
                    _logger.LogError("{Error Reason}", result.ErrorReason);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while executing command");

                if (interaction.Type == InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
