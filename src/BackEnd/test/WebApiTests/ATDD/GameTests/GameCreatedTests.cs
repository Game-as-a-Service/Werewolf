using FastEndpoints;
using System.Net;
using System.Threading.Tasks.Dataflow;
using Wsa.Gaas.Werewolf.Application;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests;
public class GameCreatedTests
{
    readonly WebApiTestServer _server = new();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _server.StartAsync();
    }

    [Test]
    [Description("""
            Issue #9
            Given: No active game in voice channel
            When: create a new game 
            Then: Success, game created

            Issue #10
            Given: An active game in voice channel
            When: create a new game in same voice channel
            Then: error 500 returned
            """)]
    public async Task CreateGameTest()
    {
        /* Arrange */
        _server.ListenOn<GameCreatedEvent>();

        var request = new CreateGameRequest()
        {
            DiscordVoiceChannelId = (ulong)new Random().Next(),
        };

        /* Act */
        var (r1, result) = await _server.Client.POSTAsync<CreateGameEndpoint, CreateGameRequest, GetGameResponse>(request);

        // 2nd Call should get 400 error
        var (response, r2) = await _server.Client.POSTAsync<CreateGameEndpoint, CreateGameRequest, GetGameResponse>(request);

        /* Assert */
        // Check Rest API Result
        result.Should().NotBeNull();
        result!.Id.Should().Be(request.DiscordVoiceChannelId);

        // Check SignalR Response
        var gameEvent = await _server.EventBuffer.ReceiveAsync();
        gameEvent.Should().NotBeNull();
        gameEvent!.Id.Should().Be(result.Id.ToString());

        // Check Database
        var repository = _server.GetRequiredService<IRepository>();
        var game = await repository.FindByDiscordChannelIdAsync(result.Id);
        game.Should().NotBeNull();
        game!.DiscordVoiceChannelId.Should().Be(result.Id);

        // Check 2nd Call Response
        response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

}