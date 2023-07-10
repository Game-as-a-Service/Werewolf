using FastEndpoints;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests
{
    public class PlayerRoleConfirmedTests
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
        public async Task ConfirmPlayerRoleTest()
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

            foreach (var player in game.Players)
            {
                var playerId = player.UserId;
                var expectedRole = player.Role!.Name;

                // Act - Rest API call
                var request = new ConfirmPlayerRoleRequest()
                {
                    DiscordVoiceChannelId = game.DiscordVoiceChannelId,
                    PlayerId = playerId,
                };

                var (response, result) = await _server.Client
                    .GETAsync<ConfirmPlayerRoleEndpoint, ConfirmPlayerRoleRequest, ConfirmPlayerRoleResponse>(request);

                // Assert API Result
                response!.Should().BeSuccessful();
                result!.PlayerId.Should().Be(playerId.ToString());
                result.Role.Should().Be(expectedRole);
            }
        }
    }
}
