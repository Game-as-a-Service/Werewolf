using System.Net;
using FastEndpoints;
using NSubstitute;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD
{
    public class HostCrateGameTests : TestsBase
    {
        private ulong _channelId;

        [Test]
        public async Task CreateGameTest()
        {
            //Init
            GivenRandomChannelId();
            HubListenOn<GameCreatedEvent>();

            //Create Game
            var (_, result) = await ExecuteCreateGame();

            result!.GameId.Should().Be(new CreateGameRequest()
                                       {
                                           DiscordVoiceChannelId = _channelId,
                                       }.DiscordVoiceChannelId.ToString());


            await WaitNetworkTransmission();

            _fakeAction.Received(1)
                       .Invoke(Arg.Is<GameVm>(o => o.Id == result.GameId));

            GetGame(ulong.Parse(result.GameId))!
               .DiscordVoiceChannelId.ToString()
               .Should().Be(result.GameId);

            //Create game in same channel
            var (response, _) = await ExecuteCreateGame();

            response!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        private void GivenRandomChannelId()
        {
            _channelId = (ulong) new Random().Next();
        }

        private async Task<(HttpResponseMessage? response, CreateGameResponse? result)> ExecuteCreateGame()
        {
            var req = new CreateGameRequest()
                      {
                          DiscordVoiceChannelId = _channelId,
                      };

            return await _httpClient.POSTAsync<CreateGameRequest, CreateGameResponse>("/games", req);
        }
    }
}