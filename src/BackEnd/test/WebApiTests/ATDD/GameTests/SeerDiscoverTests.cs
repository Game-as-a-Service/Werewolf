using FastEndpoints;
using System.Net;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.Domain.Objects.Roles;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests
{
    public class SeerDiscoverTests
    {
        readonly WebApiTestServer _server = new();

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await _server.StartAsync();
        }

        [Test]
        [Description("""
            Issue #17
            Given:
                預言家已睜眼
                1, 3, 7 玩家已淘汰
                2, 4, 5, 6, 8, 9, 10 尚存活
            When: 
                預言家選擇查驗 3 號玩家的時候
            Then: 
                回報3號玩家非存活的錯誤
            """)]

        public async Task SeerDiscoverNotSurvivedTest()
        {
            // Init
            var game = _server.CreateGameBuilder()
                .WithRandomDiscordVoiceChannel()
                .WithGameStatus(GameStatus.Created)
                .Build();
            var players = Enumerable.Range(0, 10)
                .Select(x => (ulong)x)
                .ToArray();

            game.StartGame(players);

            // Arrange
            List<int> deadNumbers = new() { 1, 3, 7 };

            ulong seerId = 99;
            foreach (var player in game.Players)
            {
                if (deadNumbers.Contains(player.PlayerNumber))
                {
                    player.IsDead = true;
                }
                if (player.Role?.GetType() == typeof(Seer))
                {
                    seerId = player.UserId;
                }
            }
            game.Status = GameStatus.SeerRoundStarted;

            var repository = _server.GetRequiredService<IRepository>();
            repository.Save(game);


            _server.ListenOn<SeerEyesOpenedEvent>();

            // Act
            var request = new DiscoverPlayerRoleRequest()
            {
                DiscordVoiceChannelId = game.DiscordVoiceChannelId,
                PlayerId = seerId,
                DiscoverPlayerNumber = 3
            };

            var (response, _) = await _server.Client.POSTAsync<DiscoverPlayerRoleEndpoint, DiscoverPlayerRoleRequest, DiscoverPlayerRoleResponse>(request);

            // Assert response hava server error
            response.Should().HaveServerError();
        }
    }
}
