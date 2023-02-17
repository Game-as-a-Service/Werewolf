﻿using FastEndpoints;
using System.Threading.Tasks.Dataflow;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests
{
    public class PlayerConfirmRoleTests
    {
        readonly WebApiTestServer _server = new();
        
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
            // TODO: We need GameBuilder to build the Game with different status correctly.
            
            var game = _server.CreateGameBuilder()
                .WithRandomDiscordVoiceChannel()
                .WithGameStatus(GameStatus.Created)
                .Build();

            var players = Enumerable.Range(0, 10)
                .Select(x => (ulong)x)
                .ToArray();
            
            game.StartGame(players);
            
            var repository = _server.GetRequiredService<IRepository>();
            repository.Save(game);

            var randomPlayer = game.Players.OrderBy(x => Guid.NewGuid()).First();
            var playerId = randomPlayer.Id;
            var expectedRole = randomPlayer.Role!.Name;

            _server.ListenOn<PlayerRoleConfirmedEvent>();


            // Act - Rest API call
            var request = new PlayerConfirmRoleRequest()
            {
                DiscordVoiceChannelId = game.DiscordVoiceChannelId,
                PlayerId = playerId,
            };

            var (response, result) = await _server.Client
                .GETAsync<PlayerConfirmRoleEndpoint, PlayerConfirmRoleRequest, PlayerConfirmRoleResponse>(request);

            // Assert API Result
            response!.EnsureSuccessStatusCode();
            result!.PlayerId.Should().Be(playerId.ToString());
            result.Role.Should().Be(expectedRole);

            // Assert SignalR
            var gameVm  = await _server.EventBuffer.ReceiveAsync();
            foreach (var playerVm in gameVm.Players)
            {
                if (playerVm.Id == playerId.ToString())
                {
                    // Only one player role is revealed
                    playerVm.Role.Should().Be(expectedRole);
                }
                else
                {
                    // Other player's role should be null
                    playerVm.Role.Should().BeNull();
                }
            }

        }
    }
}
