using FastEndpoints;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.Domain.Objects.Roles;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests;
internal class WitchUseAntidoteTests
{
    readonly WebApiTestServer _server = new();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _server.StartAsync();
    }

    [Test]
    public async Task WitchUseAntidote()
    {
        var game = _server.CreateGameBuilder()
            .WithRandomDiscordVoiceChannel()
            .WithGameStatus(GameStatus.WitchAntidoteRoundStarted)
            .WithRandomPlayers(9)
            .Build();
        var villager = game.Players.First(x => x.Role is Villager);

        villager.BuffStatus = BuffStatus.KilledByWerewolf;

        var repository = _server.GetRequiredService<IRepository>();
        repository.Save(game);

        var witch = game.Players.First(x => x.Role is Witch);

        var request = new WitchUseAntidoteRequest
        {
            DiscordVoiceChannelId = game.DiscordVoiceChannelId,
            PlayerId = witch.UserId
        };

        // Act
        var response = await _server.Client.POSTAsync<
            WitchUseAntidoteEndpoint,
            WitchUseAntidoteRequest,
            WitchUseAntidoteResponse>(request);

        // Assert 200
        response.Response.EnsureSuccessStatusCode();

        // 
        repository = _server.GetRequiredService<IRepository>();

        var actualGame = repository.FindByDiscordChannelId(game.DiscordVoiceChannelId);

        var actualVillager = actualGame!.Players.First(x => x.UserId == villager.UserId);
        var actualWitch = actualGame!.Players.First(x => x.Role is Witch);

        // 依然標記被狼殺
        ((actualVillager.BuffStatus & BuffStatus.KilledByWerewolf) == BuffStatus.KilledByWerewolf)
            .Should().BeTrue();

        // 新標記被女巫救
        ((actualVillager.BuffStatus & BuffStatus.SavedByWitch) == BuffStatus.SavedByWitch)
            .Should().BeTrue();

        // 女巫解藥已使用
        actualWitch.IsAntidoteUsed.Should().BeTrue();
    }
}
