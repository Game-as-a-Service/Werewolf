using FastEndpoints;
using System.Threading.Tasks.Dataflow;
using Wsa.Gaas.Werewolf.Application.UseCases.Games;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints.Games;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests;
internal class End2EndTests
{
    readonly WebApiTestServer _server = new();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _server.StartAsync();
    }

    [Test]
    public async Task End2End()
    {
        /* Arrange */
        _server.ListenAll();

        var createGameRequest = new GameCreateRequest()
        {
            DiscordVoiceChannelId = (ulong)new Random().Next(),
        };

        var startGameRequest = new GameStartRequest
        {
            DiscordVoiceChannelId = createGameRequest.DiscordVoiceChannelId,
            Players = _server.RandomDistinctPlayers(9),
        };

        await _server.Client.POSTAsync<GameCreateEndpoint, GameCreateRequest>(createGameRequest);
        await _server.Client.POSTAsync<GameStartEndpoint, GameStartRequest>(startGameRequest);

        var tokenSource = new CancellationTokenSource();

        List<GameVm> list = new();

        try
        {
            var gameVms = _server.EventBuffer
                .ReceiveAllAsync(tokenSource.Token);

            tokenSource.CancelAfter(1000);

            await foreach (var gameVm in gameVms)
            {
                list.Add(gameVm);
            }
        }
        catch (TaskCanceledException)
        {
        }

        list.Should().HaveCount(6);

        var expectedStatuses = new[]
        {
                nameof(GameStatus.Created),
                nameof(GameStatus.PlayerRoleConfirmationStarted),
                nameof(GameStatus.WerewolfRoundStarted),
                nameof(GameStatus.SeerRoundStarted),
                nameof(GameStatus.WitchAntidoteRoundStarted),
                nameof(GameStatus.WitchPoisonRoundStarted),
            };

        list.Select(x => x.Status)
            .Should()
            .BeEquivalentTo(expectedStatuses);
    }
}
