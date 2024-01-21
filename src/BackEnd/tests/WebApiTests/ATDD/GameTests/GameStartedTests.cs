using FastEndpoints;
using System.Net;
using System.Threading.Tasks.Dataflow;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases.Games;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints.Games;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests;
public class GameStartedTests
{
    readonly WebApiTestServer _server = new();
    readonly Random _random = new();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _server.StartAsync();
    }

    [Test]
    [Description("""
            Issue #11
            Given: Game Created
            When: Starting game with 6 Players
            Then: Error, not enough players

            Issue #11
            Given: Game Created
            When: Starting game with 20 Players
            Then: Error, too many players

            Issue #12
            Given: Game Created
            When: Starting game with duplicate Players
            Then: Error, duplicate players

            Issue #11
            Given: Game Created
            When: Starting game with 12 Players
            Then: Success, game started

            Issue #13
            Given: Game Started
            When: Starting game with 12 Players
            Then: Error, game already started
            """)]
    public async Task StartGameTest()
    {
        /* Arrange */
        _server.ListenOn<GameStartedEvent>();
        _server.ListenOn<PlayerRoleConfirmationStartedEvent>();
        _server.ListenOn<PlayerRoleConfirmationEndedEvent>();

        var gameCreated = _server.CreateGameBuilder()
            .WithRandomDiscordVoiceChannel()
            .WithGameStatus(GameStatus.Created)
            .Build();

        var gameCreated2 = _server.CreateGameBuilder()
            .WithRandomDiscordVoiceChannel()
            .WithGameStatus(GameStatus.Created)
            .Build();

        var request = new GameStartRequest
        {
            DiscordVoiceChannelId = gameCreated.DiscordVoiceChannelId,
            Players = _server.RandomDistinctPlayers(6),
        };

        /* Act & Assert */
        // too less players, expect error
        (await _server.Client.POSTAsync<GameStartEndpoint, GameStartRequest, GameGetResponse>(request))
            .Response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // too many players, expect error
        request.Players = _server.RandomDistinctPlayers(20);
        (await _server.Client.POSTAsync<GameStartEndpoint, GameStartRequest, GameGetResponse>(request))
            .Response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // duplicate players, expect error
        request.Players = RandomDuplicatePlayers(9);
        (await _server.Client.POSTAsync<GameStartEndpoint, GameStartRequest, GameStartResponse>(request))
            .Response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // 12 players
        request.Players = _server.RandomDistinctPlayers(12);
        var (r3, result) = await _server.Client.POSTAsync<GameStartEndpoint, GameStartRequest, GameStartResponse>(request);

        // Assert API response
        result!.Id.Should().Be(request.DiscordVoiceChannelId);
        result.Players.Should().HaveSameCount(request.Players);

        // Assert database
        var repository = _server.GetRequiredService<IRepository>();
        var game = await repository.FindByDiscordChannelIdAsync(result.Id);
        game.Should().NotBeNull();
        game!.DiscordVoiceChannelId.Should().Be(result.Id);
        game.Players.Should().HaveSameCount(request.Players);

        // Assert event received
        var gameVm = await _server.EventBuffer.ReceiveAsync();
        gameVm.Should().BeOfType<GameVm>();
        gameVm.Status.Should().Be(GameStatus.PlayerRoleConfirmationStarted.ToString());
        gameVm.Players.Should().HaveSameCount(request.Players);

        // game already started, expect error
        (await _server.Client.POSTAsync<GameStartEndpoint, GameStartRequest, GameGetResponse>(request))
            .Response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        request.DiscordVoiceChannelId = gameCreated2.DiscordVoiceChannelId;
        (await _server.Client.POSTAsync<GameStartEndpoint, GameStartRequest, GameGetResponse>(request))
            .Response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private ulong[] RandomDuplicatePlayers(int n)
    {
        var result = new List<ulong>();

        if (n > 0)
        {
            while (result.Count < n)
            {
                result.Add((ulong)_random.Next(0, n - 1));
            }
        }

        return result.ToArray();
    }
}