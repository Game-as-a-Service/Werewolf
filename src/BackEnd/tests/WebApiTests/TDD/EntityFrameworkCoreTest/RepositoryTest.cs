using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.EntityFrameworkCoreTest;
internal class RepositoryTest
{
    private readonly ServiceProvider _provider = new ServiceCollection()
            .AddWerewolfInfrastructure()
            .BuildServiceProvider();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _provider.Dispose();
    }

    private IRepository GetRepository()
    {
        return _provider.CreateScope().ServiceProvider.GetRequiredService<IRepository>();
    }


    [Test]
    public async Task SaveTest()
    {
        var repository = GetRepository();

        var game = new Game();
        repository.Save(game);

        game.StartGame([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        repository.Save(game);

        game.CurrentSpeakingPlayerId = game.Players.First(x => x.UserId == 3).Id;
        repository.Save(game);

        await AssertGame(game);
    }

    private async Task AssertGame(Game actual)
    {
        var repository = _provider.CreateScope().ServiceProvider.GetRequiredService<IRepository>();
        var expected = await repository.FindByDiscordChannelIdAsync(actual.DiscordVoiceChannelId);

        actual.Id.Should().Be(expected!.Id);
        actual.Status.Should().Be(expected.Status);
        actual.DiscordVoiceChannelId.Should().Be(expected.DiscordVoiceChannelId);
        actual.CurrentSpeakingPlayerId.Should().Be(expected.CurrentSpeakingPlayerId);

        AssertPlayers(actual.Players, expected.Players);
        AssertPlayers(expected.Players, actual.Players);
    }

    private static void AssertPlayers(ImmutableList<Player> actual, ImmutableList<Player> expected)
    {
        foreach (var actualPlayer in actual)
        {
            var expectedPlayer = expected.Find(x => x.UserId == actualPlayer.UserId);

            AssertPlayer(actualPlayer, expectedPlayer!);
        }
    }

    private static void AssertPlayer(Player actual, Player expected)
    {
        actual.UserId.Should().Be(expected.UserId);
        actual.PlayerNumber.Should().Be(expected.PlayerNumber);
        actual.BuffStatus.Should().Be(expected.BuffStatus);
        actual.IsDead.Should().Be(expected.IsDead);

        AssertRole(actual.Role!, expected.Role!);
    }

    private static void AssertRole(Role actual, Role expected)
    {
        actual.Id.Should().Be(expected.Id);
        actual.Name.Should().Be(expected.Name);
        actual.Faction.Should().Be(expected.Faction);
    }
}
