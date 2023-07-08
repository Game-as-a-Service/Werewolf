using FastEndpoints;
using System.Net;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests
{
    public class GameEndedTests
    {
        WebApiTestServer _server = new();

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await _server.StartAsync();
        }

        [Test]
        [Description("""
            Issue #33
            Given: A game in progress
            When: close game
            Then: Success, game is closed

            Issue #35
            Given: A game is closed
            When: close game
            Then: Error, game is already closed
            """)]

        public async Task EndGameTest()
        {
            /* Arrange */
            _server.ListenOn<GameCreatedEvent>();

            var createGameRq = new CreateGameRequest()
            {
                DiscordVoiceChannelId = (ulong)new Random().Next(),
            };

            //Create game
            var (_, createGameResponse) = await _server.Client.POSTAsync<CreateGameEndpoint, CreateGameRequest, CreateGameResponse>(createGameRq);

            var request = new EndGameRequest()
            {
                DiscordVoiceChannelId = ulong.Parse(createGameResponse!.GameId)
            };

            var (_, result) = await _server.Client.POSTAsync<EndGameEndpoint, EndGameRequest, EndGameResponse>(request);

            var (response, _) = await _server.Client.POSTAsync<EndGameEndpoint, EndGameRequest, EndGameResponse>(request);

            /* Assert */

            // Check Rest API Result
            result.Should().NotBeNull();

            // Check Database
            var repository = _server.GetRequiredService<IRepository>();
            var game = await repository.FindByDiscordChannelIdAsync(ulong.Parse(createGameResponse.GameId));
            game.Should().BeNull();

            // Check 2nd Call Response
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
