﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Wsa.Gaas.Werewolf.ChatBot.Application.Common;
using Wsa.Gaas.Werewolf.DiscordBot.Application.UseCases;
using Wsa.Gaas.Werewolf.DiscordBot.Options;

namespace Wsa.Gaas.Werewolf.DiscordBot.DiscordClients
{
    public class DiscordSocketClientAdapter : IDiscordBotClient
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordBotOptions _options;
        private readonly ILogger _logger;

        public DiscordSocketClientAdapter(
            ILogger<DiscordSocketClientAdapter> logger,
            IOptions<DiscordBotOptions> options
        )
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.AllUnprivileged,
            });

            _client.Ready += OnReadyHandler;
            _client.Log += OnLogHandler;
            _client.SlashCommandExecuted += OnSlashCommandHandler;
            _client.ButtonExecuted += OnButtonExecuted;

            _logger = logger;
            _options = options.Value;
        }

        private Task OnButtonExecuted(SocketMessageComponent arg)
        {
            arg.RespondAsync("Hello World (OnButtonExecuted)");

            return Task.CompletedTask;
        }

        public async Task StartAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _options.Token);
            await _client.StartAsync();
        }

        private async Task OnReadyHandler()
        {
            try
            {
                await CreateGlobalApplicationCommandsOnDiscord();

                _logger.LogInformation("Bot is connected!");
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                _logger.LogInformation("{json}", json);
            }
        }

        private async Task CreateGlobalApplicationCommandsOnDiscord()
        {
            var commands = new ApplicationCommandProperties[]
            {
                new SlashCommandBuilder()
                    .WithName("game")
                    .WithDescription("Werewolf Game")
                    .WithDescriptionLocalizations(new Dictionary<string, string>
                    {
                        { "zh-TW", "狼人殺遊戲" },
                    })
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("status")
                        .WithDescription("Show current Game status")
                        .WithDescriptionLocalizations(new Dictionary<string, string>
                        {
                            { "zh-TW", "顯示目前遊戲狀態" },
                        })
                        .WithType(ApplicationCommandOptionType.SubCommand)
                    )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("version")
                        .WithDescription("Show current Game version")
                        .WithDescriptionLocalizations(new Dictionary<string, string>
                        {
                            { "zh-TW", "顯示目前遊戲版本" },
                        })
                        .WithType(ApplicationCommandOptionType.SubCommand)
                    ).Build(),
            };

            await _client.BulkOverwriteGlobalApplicationCommandsAsync(commands);
        }

        private Task OnLogHandler(LogMessage msg)
        {
            _logger.LogInformation("{msg}", msg.ToString());
            return Task.CompletedTask;
        }

        private async Task OnSlashCommandHandler(SocketSlashCommand command)
        {
            var subCommand = command.Data.Options.FirstOrDefault()?.Name;

            if (subCommand == "version")
            {
                var version = await new GameVersionGetUseCase()
                    .ExecuteAsync(new GameVersionGetRequest());

                var builder = new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .WithCurrentTimestamp()
                    .WithTitle("狼人殺版本")
                    .WithDescription(version)
                    ;

                await command.RespondAsync(
                    embeds: new[] { builder.Build() }
                );
            }
            else if (subCommand == "status")
            {
                if (command.Channel is SocketVoiceChannel)
                {
                    var componentBuilder = new ComponentBuilder()
                    .AddRow(new ActionRowBuilder()
                        .WithButton("Create Game", "create-game", ButtonStyle.Primary)
                    )
                    ;

                    await command.RespondAsync(
                        "遊戲狀態還沒做",
                        components: componentBuilder.Build()
                    );
                }
                else
                {
                    var text = $"{command.Channel.Name} 是 ${command.Channel.GetChannelType()} 頻道 , 請在語音頻道使用此指令";

                    await command.RespondAsync(
                        text,
                        ephemeral: true
                    );
                }
            }
            else
            {
                await command.RespondAsync(
                    $"Unknown Command: {subCommand}"
                );
            }
        }
    }
}