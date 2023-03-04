using System.Net;
using FastEndpoints;
using NSubstitute;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD
{
    [TestFixture]
    public class HostStartGameTests : TestsBase
    {
        [Test]
        [TestCase(8, HttpStatusCode.InternalServerError, TestName = "less_than_min_limit")]
        [TestCase(9, HttpStatusCode.OK, TestName = "success_9")]
        [TestCase(12, HttpStatusCode.OK, TestName = "success_12")]
        [TestCase(13, HttpStatusCode.InternalServerError, TestName = "exceed_max_limit")]
        public async Task player_count(int playerCount, HttpStatusCode expectedStatusCode)
        {
            //Given
            var game = GivenGame(gameStatus: GameStatus.Created);
            HubListenOn(nameof(GameStartedEvent));

            //When
            var (response, _) = await ExecuteStartGame(game.DiscordVoiceChannelId,
                                                       RandomDistinctPlayers(playerCount));

            //Then
            response!.StatusCode.Should().Be(expectedStatusCode);

            await WaitNetworkTransmission();

            switch (expectedStatusCode)
            {
                case HttpStatusCode.OK:
                    _fakeAction.Received(1)
                               .Invoke(Arg.Is<GameVm>(o => o.Id == game.DiscordVoiceChannelId.ToString()
                                                        && o.Status == GameStatus.Started.ToString()));

                    break;
                case HttpStatusCode.InternalServerError:
                    _fakeAction.DidNotReceive()
                               .Invoke(Arg.Is<GameVm>(o => o.Id == game.DiscordVoiceChannelId.ToString()
                                                        && o.Status == GameStatus.Started.ToString()));

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expectedStatusCode), expectedStatusCode, null);
            }
        }


        [Test]
        public async Task duplicate_player()
        {
            //Given
            var game = GivenGame(gameStatus: GameStatus.Created);
            HubListenOn(nameof(GameStartedEvent));

            //When
            var (response, _) = await ExecuteStartGame(game.DiscordVoiceChannelId,
                                                       1, 1, 2, 3, 4, 5, 6, 7);

            //Then
            response!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            _fakeAction.DidNotReceive()
                       .Invoke(Arg.Is<GameVm>(o => o.Id == game.DiscordVoiceChannelId.ToString()
                                                && o.Status == GameStatus.Started.ToString()));
        }

        [Test]
        public async Task already_started()
        {
            //Given
            var game = GivenGame(gameStatus: GameStatus.Created);
            HubListenOn(nameof(GameStartedEvent));

            //When
            var (response, _) = await ExecuteStartGame(game.DiscordVoiceChannelId,
                                                       RandomDistinctPlayers(8));

            //Then
            response!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            _fakeAction.DidNotReceive()
                       .Invoke(Arg.Is<GameVm>(o => o.Id == game.DiscordVoiceChannelId.ToString()
                                                && o.Status == GameStatus.Started.ToString()));
        }

        private async Task<(HttpResponseMessage? response, StartGameResponse? result)> ExecuteStartGame(ulong discordVoiceChannelId, params ulong[] randomDistinctPlayers)
        {
            return await _httpClient.POSTAsync<StartGameRequest, StartGameResponse>($"/games/{discordVoiceChannelId}/start",
                                                                                    new StartGameRequest
                                                                                    {
                                                                                        DiscordVoiceChannelId = discordVoiceChannelId,
                                                                                        Players = randomDistinctPlayers,
                                                                                    });
        }

        private Game GivenGame(GameStatus gameStatus)
        {
            return _gameBuilder.WithRandomDiscordVoiceChannel()
                               .WithGameStatus(gameStatus)
                               .Build();
        }
    }
}