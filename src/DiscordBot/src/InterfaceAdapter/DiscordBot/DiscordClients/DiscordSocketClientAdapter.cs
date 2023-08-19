using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Json;
using Wsa.Gaas.Werewolf.ChatBot.Application.Common;
using Wsa.Gaas.Werewolf.DiscordBot.Dtos;
using Wsa.Gaas.Werewolf.DiscordBot.Options;
using Wsa.Gaas.Werewolf.DiscordBot.ViewModels;

namespace Wsa.Gaas.Werewolf.DiscordBot.DiscordClients;

public class DiscordSocketClientAdapter : IDiscordBotClient
{
    private readonly DiscordBotOptions _botOptions;
    private readonly BackendApiEndpointOptions _apiOptions;
    private readonly Random _random = new();

    // TODO: remove
    private readonly Dictionary<ulong, List<ulong>> _allJoinedPlayers = new();

    private readonly ILogger _logger;
    private readonly IServiceProvider _services;
    private readonly DiscordSocketClient _client;
    private readonly BackendApi _backendApi;
    private readonly InteractionService _interactionService;
    private readonly HubConnection _hubConnection;

    public DiscordSocketClientAdapter(
        ILogger<DiscordSocketClientAdapter> logger,
        IOptions<DiscordBotOptions> botOptions,
        IOptions<BackendApiEndpointOptions> apiOptions,

        BackendApi backendApi,
        DiscordSocketClient discordSocketClient,

        IServiceProvider services        
    )
    {
        _services = services;
        _logger = logger;

        // 必要的
        _client = discordSocketClient;
        _client.InteractionCreated += OnInteractionCreatedHandler;
        _client.Ready += OnReadyHandler;
        _client.Log += OnLogHandler;
        _interactionService = new InteractionService(_client);

        _backendApi = backendApi;
        _botOptions = botOptions.Value;
        _apiOptions = apiOptions.Value;

        _hubConnection = new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl(_apiOptions.SignalR)
            .Build();

        // TODO: too many events
        var eventNames = new[]
        {
            "GameCreatedEvent",
            "GameStartedEvent",
            "PlayerRoleConfirmationStartedEvent",
            "PlayerRoleConfirmationEndedEvent",
        };

        foreach (var eventName in eventNames)
        {
            _hubConnection.On<GameVm>(
                eventName, 
                gameVm => OnReceive(eventName, gameVm)
            );
        }
    }

    private async Task OnReceive(string eventName, GameVm game)
    {
        _logger.LogInformation("{eventName} {@game}", eventName, game);

        if (eventName == "PlayerRoleConfirmationEndedEvent")
        {
            // get channel

            if (await _client.GetChannelAsync(ulong.Parse(game.Id)) is SocketVoiceChannel channel)
            {
                var gameDto = new GameDto()
                {
                    Id = ulong.Parse(game.Id),
                    Players = game.Players.Select(x => new PlayerDto
                    {
                        UserId = ulong.Parse(x.Id),
                        PlayerNumber = x.PlayerNumber,
                        Role = x.Role ?? string.Empty
                    }).ToList(),
                    Status = Enum.Parse<GameStatus>(game.Status),
                };

                await channel.SendMessageAsync(
                    text: "# 確認角色身分已結束\n現在天黑請閉眼\n狼人請睜眼。",
                    embeds: new[]
                    {
                        BuildGameEmbed(channel, gameDto),
                    }
                );
            }
        }
    }

    private async Task OnButtonExecuted(SocketMessageComponent arg)
    {
        var channel = (SocketVoiceChannel)arg.Channel;
        var channelId = arg.Channel.Id;
        var userId = arg.User.Id;
        string? message = null;

        //if (arg.Data.CustomId == "btn-join-game")
        //{
        //    message = $"{arg.User.Mention}已加入";
        //    UpdateGamePlayer(channelId, userId, true);
        //}
        //else if (arg.Data.CustomId == "btn-leave-game")
        //{
        //    message = $"{arg.User.Mention}已離開";
        //    UpdateGamePlayer(channelId, userId, false);
        //}
        //else if (arg.Data.CustomId == "btn-create-game")
        //{
        //    message = "遊戲已新增";
        //    await CreateGame(channel, arg.User);
        //}
        if (arg.Data.CustomId == "btn-start-game")
        {
            message = "# 遊戲開始囉!\n請確認角色身分!";
            await StartGame(channelId);
        }
        else if (arg.Data.CustomId == "btn-confirm-player-role")
        {
            await ConfirmPlayerRole(arg);
        }
        else
        {
            _logger.LogError("cannot find custom id {customId}", arg.Data.CustomId);
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

    

    private Color RandomColor()
    {
        return new Color(_random.Next(256), _random.Next(256), _random.Next(256));
    }

    public async Task StartAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _botOptions.Token);
        await _client.StartAsync();
        await _hubConnection.StartAsync();
    }

    private async Task OnReadyHandler()
    {
        try
        {
            // Ready 後才開始新增 SlashCommand
            await _interactionService.AddModulesAsync(
                Assembly.GetEntryAssembly(),
                _services
            );

#if DEBUG
            await _interactionService.RegisterCommandsToGuildAsync(1066997588961787955, true);
#else
            await _interactionService.RegisterCommandsGloballyAsync();
#endif

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

    private async Task OnInteractionCreatedHandler(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_client, interaction);

            var result = await _interactionService.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
            {
                _logger.LogError("{ErrorReason}", result.ErrorReason);
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