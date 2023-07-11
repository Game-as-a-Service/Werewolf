using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Wsa.Gaas.Werewolf.ChatBot.Application.Common;
using Wsa.Gaas.Werewolf.DiscordBot.Application.UseCases;
using Wsa.Gaas.Werewolf.DiscordBot.Dtos;
using Wsa.Gaas.Werewolf.DiscordBot.Options;

namespace Wsa.Gaas.Werewolf.DiscordBot.DiscordClients;

public class DiscordSocketClientAdapter : IDiscordBotClient
{
    private readonly DiscordSocketClient _client;
    private readonly DiscordBotOptions _options;
    private readonly ILogger _logger;
    private readonly BackendApi _backendApi;
    private readonly Dictionary<ulong, List<ulong>> _allJoinedPlayers = new();
    private readonly Random _random = new();

    public DiscordSocketClientAdapter(
        ILogger<DiscordSocketClientAdapter> logger,
        IOptions<DiscordBotOptions> options,
        BackendApi backendApi
    )
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.MessageContent
                | GatewayIntents.AllUnprivileged
                | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true,
        });

        _client.Ready += OnReadyHandler;
        _client.Log += OnLogHandler;
        _client.SlashCommandExecuted += async command =>
        {
            try
            {
                await OnSlashCommandHandler(command);
            }
            catch (Exception ex)
            {
                await command.Channel.SendMessageAsync(
                    embed: new EmbedBuilder()
                        .WithColor(Color.Red)
                        .WithDescription(ex.Message)
                        .Build()
                );
            }
        };
        _client.ButtonExecuted += OnButtonExecuted;

        _logger = logger;
        _backendApi = backendApi;
        _options = options.Value;
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
                try
                {
                    var gameDto = await _backendApi.GetGame(channel.Id);

                    if (gameDto == null)
                    {
                        await command.RespondAsync(
                            "沒有遊戲正在進行中。",
                            components: BuildButtons(channel, gameDto)
                        );
                    }
                    else
                    {
                        await command.RespondAsync(
                            embed: BuildGameEmbed(channel, gameDto),
                            components: BuildButtons(channel, gameDto)
                        );
                    }

                }
                catch (Exception ex)
                {
                    await command.RespondAsync(
                        $"""
                    ```
                    {ex.Message}
                    ```
                    """
                    );
                }
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

        else if (subCommand == "invite")
        {
            if (command.Channel is SocketVoiceChannel channel)
            {
                var users = channel.Guild.Users; // 2 Users
                var numberOfPlayers = command.Data.Options.First().Options.First();

                if (numberOfPlayers.Value is long n)
                {
                    RandomGame(channel.Id, (int)n, users);
                }
                var gameDto = new GameDto { Id = channel.Id };
                await command.RespondAsync(
                    embeds: new[]
                    {
                        BuildGameEmbed(channel, gameDto)
                    },
                    components: BuildButtons(channel, gameDto)
                );
            }
        }
        else if (subCommand == "create")
        {
            if (command.Channel is SocketVoiceChannel channel)
            {
                var gameDto = await CreateGame(channel, command.User);

                await command.RespondAsync(
                    embeds: new[]
                    {
                        BuildGameEmbed(channel, gameDto),
                    },
                    components: BuildButtons(channel, gameDto)
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

    private async Task<GameDto> CreateGame(SocketVoiceChannel channel, IUser user)
    {
        // Data Transfer Object
        var gameDto = await _backendApi.CreateGame(channel.Id)
            ?? await _backendApi.GetGame(channel.Id);

        if (_allJoinedPlayers.ContainsKey(gameDto!.Id) == false)
        {
            _allJoinedPlayers[gameDto.Id] = new List<ulong>
            {
                user.Id,
            };
        }

        return gameDto;
    }

    private async Task OnButtonExecuted(SocketMessageComponent arg)
    {
        var channel = (SocketVoiceChannel)arg.Channel;
        var channelId = arg.Channel.Id;
        var userId = arg.User.Id;
        string? message = null;

        if (arg.Data.CustomId == "btn-join-game")
        {
            message = $"{arg.User.Mention}已加入";
            UpdateGamePlayer(channelId, userId, true);
        }
        else if (arg.Data.CustomId == "btn-leave-game")
        {
            message = $"{arg.User.Mention}已離開";
            UpdateGamePlayer(channelId, userId, false);
        }
        else if (arg.Data.CustomId == "btn-create-game")
        {
            message = "遊戲已新增";
            await CreateGame(channel, arg.User);
        }
        else if (arg.Data.CustomId == "btn-start-game")
        {
            message = "遊戲開始囉!\n請確認角色身分!";
            await StartGame(channelId);
        }
        else if (arg.Data.CustomId == "btn-confirm-player-role")
        {
            await ConfirmPlayerRole(arg);
        }

        var gameDto = await _backendApi.GetGame(channelId) ?? new GameDto();

        await arg.UpdateAsync(prop =>
        {
            prop.Content = message;
            prop.Embeds = new[]
            {
                BuildGameEmbed(channel, gameDto),
            };

            prop.Components = BuildButtons(channel, gameDto);
        });
    }

    private async Task ConfirmPlayerRole(SocketMessageComponent arg)
    {
        var channelId = arg.ChannelId;
        var userId = arg.User.Id;

        var role = await _backendApi.ConfirmPlayerRole(channelId, userId);

        await arg.RespondAsync(
            $"# 你是 ... **{role}**", ephemeral: true
        );
    }

    private async Task<GameDto> StartGame(ulong channelId)
    {
        var joinedPlayers = _allJoinedPlayers[channelId];

        var gameDto = await _backendApi.StartGame(channelId, joinedPlayers);

        return gameDto;
    }

    private void UpdateGamePlayer(ulong channelId, ulong userId, bool join)
    {
        if (_allJoinedPlayers.ContainsKey(channelId) == false)
        {
            _allJoinedPlayers[channelId] = new();
        }

        var joinedPlayers = _allJoinedPlayers[channelId];

        if (join && joinedPlayers.Contains(userId) == false && joinedPlayers.Count < 12)
        {
            joinedPlayers.Add(userId);
        }
        else if (join == false)
        {
            joinedPlayers.Remove(userId);
        }
    }

    private void RandomGame(ulong channelId, int n, IEnumerable<IUser> users)
    {
        if (_allJoinedPlayers.ContainsKey(channelId) == false)
        {
            _allJoinedPlayers[channelId] = new();
        }

        var joinedPlayers = _allJoinedPlayers[channelId];

        joinedPlayers.AddRange(
            users
                .Where(x => x.IsBot == false)
                .Where(x => joinedPlayers.Contains(x.Id) == false)
                .Take(n - joinedPlayers.Count)
                .Select(x => x.Id)
        );
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
            var json = JsonSerializer.Serialize(exception.Errors, new JsonSerializerOptions
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

                // /game invite {numberOfPlayers}
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("invite")
                    .WithDescription("Invite user")
                    .WithDescriptionLocalizations(new Dictionary<string, string>
                    {
                        { "zh-TW", "邀請其他使用者" },
                    })
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("number-of-players")
                        .WithDescription("Number of Players")
                        .WithDescriptionLocalizations(new Dictionary<string, string>
                        {
                            { "zh-TW", "遊戲人數 9 ~ 12 人" },
                        })
                        .WithType(ApplicationCommandOptionType.Integer)
                        .WithMinValue(9)
                        .WithMaxValue(12)
                        .WithRequired(true)
                    )
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
        // _interactionService.RegisterCommandsToGuildAsync(1030466547325607936);
        foreach (var command in commands)
        {
            await _client.CreateGlobalApplicationCommandAsync(command);
        }
        await _client.BulkOverwriteGlobalApplicationCommandsAsync(commands);
    }

    private Task OnLogHandler(LogMessage msg)
    {
        _logger.LogInformation("{msg}", msg.ToString());
        return Task.CompletedTask;
    }

    private Embed BuildGameEmbed(SocketVoiceChannel channel, GameDto gameDto)
    {
        var joinedPlayers = gameDto.Status == GameStatus.PlayerRoleConfirmationStarted
            ? gameDto.Players.Select(x => x.UserId)
            : _allJoinedPlayers[channel.Id];

        var players = "";
        for (var i = 0; i < 12; i++)
        {
            var user = channel.Guild.Users.FirstOrDefault(x => x.Id == joinedPlayers.ElementAtOrDefault(i));
            players += $"[{user?.Mention ?? " "}]\n";
        }

        return new EmbedBuilder()
            .WithColor(RandomColor())
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Slot")
                    .WithValue(string.Join("\n", Enumerable.Range(1, 12).Select(x => $"[{x}]")))
                    .WithIsInline(true)
                    ,

                new EmbedFieldBuilder()
                    .WithName("Players")
                    .WithValue(players)
                    .WithIsInline(true)
            )
            .Build();
    }

    private MessageComponent BuildButtons(SocketVoiceChannel channel, GameDto? gameDto)
    {


        var builder = new ComponentBuilder();

        if (gameDto == null)
        {
            builder = builder.WithButton(
                "開新遊戲",
                "btn-create-game",
                ButtonStyle.Primary
            );
        }
        else if (gameDto.Status == GameStatus.PlayerRoleConfirmationStarted)
        {
            builder = builder.WithButton(
                "確認我的角色身分",
                "btn-confirm-player-role",
                ButtonStyle.Primary
            );
        }
        else
        {
            var canStart = _allJoinedPlayers[channel.Id].Count >= 9;
            builder = builder.WithButton(
                    "加入遊戲",
                    "btn-join-game",
                    ButtonStyle.Primary
                )
                .WithButton(
                    "離開遊戲",
                    "btn-leave-game",
                    ButtonStyle.Danger
                )
                .WithButton(
                    "開始遊戲",
                    "btn-start-game",
                    canStart ? ButtonStyle.Success : ButtonStyle.Secondary,
                    disabled: !canStart
                );
        }

        return builder.Build();
    }
}