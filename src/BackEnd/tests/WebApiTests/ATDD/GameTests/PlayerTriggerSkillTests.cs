using FastEndpoints;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.Domain.Objects.Roles;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests;
internal class PlayerTriggerSkillTests
{
    readonly WebApiTestServer _server = new();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _server.StartAsync();
    }

    [Test]
    [Description("""
            Given:
            9 位玩家 (1,2,3,4,5,6,7,8,9)
            3 號玩家是獵人
            3 號玩家出局後

            When:
            3 號玩家選擇 4 號玩家一起出局

            Then:
            4 號玩家出局

            """)]
    public async Task PlayerTriggerTest()
    {
        // Arrange
        var players = Enumerable.Range(0, 10)
            .Select(x => (ulong)x)
            .ToArray();

        var game = _server.CreateGameBuilder()
            .WithRandomDiscordVoiceChannel()
            .WithGameStatus(GameStatus.Created)
            .Build();

        game.StartGame(players);

        // 3 號玩家是獵人
        // 3 號玩家出局後
        var hunter = game.Players.First(x => x.Role is Hunter);
        hunter.IsDead = true;

        // target Player
        var targetPlayer = game.Players.First(x => x.UserId != hunter.UserId);

        // 宣布昨晚結果
        game.Status = GameStatus.LastNightResultAnnounced;

        var request = new PlayerTriggerSkillRequest
        {
            DiscordVoiceChannelId = game.DiscordVoiceChannelId,
            PlayerId = hunter.UserId,
            TargetPlayerId = targetPlayer.UserId,
        };

        // Act => 呼叫 API
        var (response, result) = await _server.Client.POSTAsync<PlayerTriggerSkillEndpoint, PlayerTriggerSkillRequest, PlayerTriggerSkillResponse>(request);

        // Assert Http Response
        response.EnsureSuccessStatusCode();

        // Assert Repository
        var repository = _server.GetRequiredService<IRepository>();
        var actualGame = await repository.FindByDiscordChannelIdAsync(game.DiscordVoiceChannelId);
        actualGame!.Should().NotBeNull();

        // Target Player 真的死掉
        var actualTargetPlayer = actualGame!.Players.FirstOrDefault(x => x.UserId == targetPlayer.UserId);
        actualTargetPlayer!.IsDead.Should().BeTrue();

    }
}

