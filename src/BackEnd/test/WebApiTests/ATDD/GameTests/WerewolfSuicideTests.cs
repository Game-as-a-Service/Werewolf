using FastEndpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests
{
    internal class WerewolfSuicideTests
    {
        readonly WebApiTestServer _server = new();

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await _server.StartAsync();
        }

        [Test]
        [Description("""
            Given
            * 遊戲在白天發言環節
            * 遊戲有 9 人，Player 1,2,3,4,5,6,7,8,9
            * Player 1 是狼人
        
            When
            * Player 1 狼人選擇自爆

            Then
            * 公布狼人身分
            * 結束白天發言環節, 直接進入黑夜

        """)]
        public async Task WerewolfSuicideTest()
        {
            // Arrange
            var game = _server.CreateGameBuilder()
                 .WithRandomDiscordVoiceChannel()
                 .WithGameStatus(GameStatus.Started)
                 .WithRandomPlayers(9)
                 .Build();

            // 狼人
            var werewolf = game.Players
                .Where(x => x.Role is Domain.Objects.Roles.Werewolf)
                .First();

            var request = new WerewolfSuicideRequest
            {
                DiscordVoiceChannelId = game.DiscordVoiceChannelId,
                PlayerId = werewolf.UserId,
            };

            var httpClient = _server.Client;

            // Act
            var result = await httpClient
               // FastEndpoint 套件擴充
               .POSTAsync<WerewolfSuicideEndpoint, WerewolfSuicideRequest, WerewolfSuicideResponse>(request);

            // Assert
            result.Response.Should().HaveStatusCode(HttpStatusCode.OK);

            // 資料庫
            var repository = _server.GetRequiredService<IRepository>();
            var actualGame = await repository.FindByDiscordChannelIdAsync(game.DiscordVoiceChannelId);

            var actualWerewolf = actualGame!.Players.First(x => x.UserId == werewolf.UserId);

            // FluentAssert
            // 狼人真的死了
            actualWerewolf.IsDead.Should().BeTrue();
        }
    }
}
