using FastEndpoints;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Objects.Roles;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;
using Moq;
using Microsoft.Extensions.Options;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests
{
    public class WitchUsePoisonTests
    {
        readonly WebApiTestServer _server = new();

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await _server.StartAsync();
        }

        [Test]
        public async Task WitchUsePoison()
        {
            // Arrange
            var game = _server.CreateGameBuilder()
                .WithRandomDiscordVoiceChannel()
                .WithGameStatus(GameStatus.WitchRoundStarted)
                .WithRandomPlayers(9)
                .Build();

            var witch = game.Players.First(x => x.Role is Witch);

            var targetPlayer = game.Players.First(x => x.IsDead == false);

            var request = new WitchUsePoisonRequest
            {
                DiscordVoiceChannelId = game.DiscordVoiceChannelId,
                PlayerId = witch.UserId,
                TargetPlayerId = targetPlayer.UserId,
            };
            
            // Act
            var response = await _server.Client.POSTAsync<
                WitchUsePoisonEndpoint,
                WitchUsePoisonRequest,
                WitchUsePoisonResponse>(request);

            // Assert 200
            response.Response.EnsureSuccessStatusCode();

            // 
            var repository = _server.GetRequiredService<IRepository>();

            var actualGame = repository.FindByDiscordChannelId(game.DiscordVoiceChannelId);

            var actualTargetPlayer = actualGame!.Players.First(x => x.UserId == targetPlayer.UserId);
            var actualWitch = actualGame!.Players.First(x => x.Role is Witch);

            // 新標記被女巫毒
            ((actualTargetPlayer.BuffStatus & BuffStatus.KilledByWitch) == BuffStatus.KilledByWitch)
                .Should().BeTrue();

            // 女巫毒藥已使用
            actualWitch.IsPoisonUsed.Should().BeTrue();

        }
    }
}
