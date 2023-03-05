using System.Net;
using FastEndpoints;
using NSubstitute;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Endpoints.Response;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD
{
    public class HostCrateGameTests : TestsBase
    {
        private long _channelId;

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
                                           RoomId = _channelId,
                                       }.RoomId.ToString());


            await WaitNetworkTransmission();

            FakeAction.Received(1)
                       .Invoke(Arg.Is<GameVm>(o => o.Id == result.GameId));

            GetGame(long.Parse(result.GameId))!
               .RoomId.ToString()
               .Should().Be(result.GameId);

            //Create game in same channel
            var (response, _) = await ExecuteCreateGame();

            response!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        private void GivenRandomChannelId()
        {
            _channelId = new Random().Next();
        }

        private async Task<(HttpResponseMessage? response, CreateGameResponse? result)> ExecuteCreateGame()
        {
            var req = new CreateGameRequest()
                      {
                          RoomId = _channelId,
                      };

            return await HttpClient.POSTAsync<CreateGameRequest, CreateGameResponse>("/games", req);
        }
    }
}