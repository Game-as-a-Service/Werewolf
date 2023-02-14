using FastEndpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests
{
    public class PlayerConfirmRoleTests
    {
        WebApiTestServer _server = new();
        Random _random = new();

        /// <summary>
        /// 每個Test執行前，都會執行一次
        /// </summary>
        /// <returns></returns>
        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await _server.StartAsync();
        }
        
        /// <summary>
        /// Test
        /// </summary>
        [Test]
        [Description("""
            假設:
            確認角色身分環節已開始

            當:
            玩家確認角色身分

            就
            告知玩家角色身分
            """
            )]
        public async Task PlayerConfirmRoleTest() 
        {
            // Arrange - Set up game in database
            var game = _server.CreateGameBuilder()
                .WithRandomDiscordVoiceChannel()
                //.WithRandomPlayers(10)
                .WithGameStatus(GameStatus.Created)
                .Build();
            var players = Enumerable.Range(0, 10)
                .Select(x => (ulong)x)
                .ToArray();
            game.StartGame(players);
            var repository = _server.GetRequiredService<IRepository>();
            repository.Save(game);

            var playerId = game.Players.First().Id;
            var expectedRole = game.Players.First().Role!.Name;

            // Act - Rest API call
            var request = new PlayerConfirmRoleRequest()
            {
                DiscordVoiceChannelId = game.DiscordVoiceChannelId,
                PlayerId = playerId,
            };

            var (response, result) = await _server.Client.POSTAsync<PlayerConfirmRoleEndpoint, PlayerConfirmRoleRequest, PlayerConfirmRoleResponse>(request);

            // Asert 
            response!.EnsureSuccessStatusCode();
            result!.PlayerId.Should().Be(playerId.ToString());
            result!.Role.Should().Be(expectedRole);




        }
    }
}
