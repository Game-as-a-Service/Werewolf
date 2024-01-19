using Bogus;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.DomainTest;
public class GameTest
{
    [Test]
    public void CreateGameTest()
    {
        // Arrange 
        var discordChannelId = new Faker().Random.ULong(1);

        // Act
        var game = new Game(discordChannelId);

        // Assert
        game.Status.Should().Be(GameStatus.Created);
        game.DiscordVoiceChannelId.Should().Be(discordChannelId);
    }

    [Test]
    public void StartGameTest()
    {
        // Arrange 
        var faker = new Faker();
        var discordChannelId = faker.Random.ULong(1);
        var game = new Game(discordChannelId);
        var numberOfPlayers = faker.PickRandom(Enumerable.Range(9, 4));
        var playerIds = Enumerable.Range(0, numberOfPlayers)
            .Select(_ => faker.Random.ULong(1))
            .ToArray();

        // Act
        game.StartGame(playerIds);

        // Assert
        game.Status.Should().Be(GameStatus.PlayerRoleConfirmationStarted);
        game.DiscordVoiceChannelId.Should().Be(discordChannelId);
        AssertPlayers(game, playerIds);
    }

    private static void AssertPlayers(Game game, ulong[] playerIds)
    {
        var numberOfPlayers = playerIds.Length;
        var expectedRoles = Game.GetRoles(numberOfPlayers);

        game.Players.Should().HaveCount(numberOfPlayers);
        game.Players.Should().AllSatisfy(p =>
        {
            p.IsDead.Should().BeFalse();
            p.BuffStatus.Should().Be(BuffStatus.None);
            p.PlayerNumber.Should().BeInRange(1, numberOfPlayers);
        });
        game.Players.Select(x => x.PlayerNumber).Should().BeEquivalentTo(Enumerable.Range(1, numberOfPlayers));
        game.Players.Select(x => x.UserId).Should().BeEquivalentTo(playerIds);
        game.Players.Select(x => x.Role).Should().BeEquivalentTo(expectedRoles);
    }
}
