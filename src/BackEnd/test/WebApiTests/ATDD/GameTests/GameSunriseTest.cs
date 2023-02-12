using System.Net;
using System.Threading.Tasks.Dataflow;
using Application.UseCases;
using Domain.Events;
using FastEndpoints;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace WebApiTests.ATDD.GameTests
{
    public class GameSunriseTest
    {
        WebApiTestServer _server = new(); 

        Random _random = new();


        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await _server.StartAsync();
        }

        [Test]
        [Description("""
            Issue #14
            Given: 白天發言環節
            When: 狼人選擇自爆
            Then: 公布狼人身分，結束白天發言環節, 直接進入黑夜
            """)]
        public async Task WerewolfSuicideTest()
        {
            /* Arrange */
            _server.ListenOn<SunriseEvent>();

            // var gameCreated = _server.StartAsync();

            var gameCreated = _server.CreateGameBuilder()
                .WithRandomDiscordVoiceChannel()
                .WithGameStatus(GameStatus.Created)
                .Build();

            var request = new SunriseRequest
            {
                DiscordVoiceChannelId = gameCreated.DiscordVoiceChannelId,
                Players = RandomDistinctPlayers(12)
            };

            /* Act */
            (await _server.Client.POSTAsync<SunriseEndpoint, SunriseRequest, SunriseResponse>(request))
                .response!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            /* Assert */
        }

        private ulong[] RandomDistinctPlayers(int n)
        {
            var result = new HashSet<ulong>();

            if (n > 0)
            {
                while (result.Count < n)
                {
                    result.Add((ulong)_random.Next());
                }
            }

            return result.ToArray();
        }
    }
}