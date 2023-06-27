using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Wsa.Gaas.Werewolf.ChatBot.Application.Common;
using Wsa.Gaas.Werewolf.DiscordBot.Application.UseCases;
using Wsa.Gaas.Werewolf.DiscordBot.Options;

namespace Wsa.Gaas.Werewolf.DiscordBot.DiscordClients;

public class DiscordSocketClientAdapter : IDiscordBotClient
{
    private readonly DiscordSocketClient _client;
    private readonly DiscordBotOptions _options;
    private readonly ILogger _logger;
    private readonly Dictionary<ulong, List<ulong>> _allJoinedPlayers = new();
    private readonly Random _random = new();

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

    private async Task OnButtonExecuted(SocketMessageComponent arg)
    {
        if (arg.Data.CustomId == "btn-join-game")
        {
            if (_allJoinedPlayers.ContainsKey(arg.Channel.Id) == false)
            {
                _allJoinedPlayers[arg.Channel.Id] = new();
            }

            var joinedPlayers = _allJoinedPlayers[arg.Channel.Id];
            if (joinedPlayers.Contains(arg.User.Id) == false)
            {
                joinedPlayers.Add(arg.User.Id);
            }

            await arg.UpdateAsync(prop =>
            {
                prop.Embeds = new[]
                {
                    new EmbedBuilder()
                        .WithDescription(BuildDescription((SocketVoiceChannel)arg.Channel))
                        .WithColor(RandomColor())
                        .Build()
                };
            });
        }
        else if (arg.Data.CustomId == "btn-leave-game")
        {
            if (_allJoinedPlayers.ContainsKey(arg.Channel.Id))
            {
                var joinedPlayers = _allJoinedPlayers[arg.Channel.Id];

                joinedPlayers.Remove(arg.User.Id);
            }

            await arg.UpdateAsync(prop =>
            {
                prop.Embeds = new[]
                {
                    new EmbedBuilder()
                        .WithDescription(BuildDescription((SocketVoiceChannel)arg.Channel))
                        .WithColor(RandomColor())
                        .Build()
                };
            });

        }
    }

    private Color RandomColor()
    {
        return new Color(_random.Next(256), _random.Next(256), _random.Next(256));
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
            var json = JsonSerializer.Serialize(exception.Errors,new JsonSerializerOptions
            {
                WriteIndented = true,
            });

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
                    .WithName("create")
                    .WithDescription("Create new game")
                    .WithDescriptionLocalizations(new Dictionary<string, string>
                    {
                        { "zh-TW", "新增遊戲" },
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
                )
                
                .Build(),
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
            if (command.Channel is SocketVoiceChannel channel)
            {
                // 呼叫 後端 Creat Game API 
                var backendApi = new BackendApi();

                var jsonString = JsonSerializer.Serialize(
                    await backendApi.GetGame(channel.Id), 
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    }
                );

                await command.RespondAsync(
                    $"""
                    ```
                    {jsonString}
                    ```
                    """
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
        else if (subCommand == "create")
        {
            if (command.Channel is SocketVoiceChannel channel)
            {
                // 呼叫 後端 Creat Game API 
                var backendApi = new BackendApi();

                // Data Transfer Object
                var gameDto = await backendApi.CreateGame(channel.Id)
                    ?? await backendApi.GetGame(channel.Id);

                if (_allJoinedPlayers.ContainsKey(gameDto!.Id) == false)
                {
                    _allJoinedPlayers[gameDto.Id] = new List<ulong>();
                }

                var component = new ComponentBuilder()
                        .WithButton(
                            "加入遊戲",
                            "btn-join-game",
                            ButtonStyle.Primary
                        )
                        .WithButton(
                            "離開遊戲",
                            "btn-leave-game",
                            ButtonStyle.Danger
                        )
                        .Build()
                        ;
                string description = BuildDescription(channel);

                await command.RespondAsync(
                    embeds: new[]
                    {
                        new EmbedBuilder()
                            .WithTitle("遊戲已新增")
                            .WithDescription(description)
                            .WithColor(Color.Orange)
                            .Build()
                    },
                    components: component
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

    private string BuildDescription(SocketVoiceChannel channel)
    {
        var description = "";
        var joinedPlayers = _allJoinedPlayers[channel.Id];
        for (var i = 0; i < 12; i++)
        {
            var user = channel.Guild.Users.FirstOrDefault(x => x.Id == joinedPlayers.ElementAtOrDefault(i));
            description += $"Player #{i + 1}: " + user?.Mention + Environment.NewLine;
        }

        return description;
    }
}