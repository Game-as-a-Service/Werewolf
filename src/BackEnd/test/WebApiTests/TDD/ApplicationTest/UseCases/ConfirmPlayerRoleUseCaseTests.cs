using Microsoft.Extensions.DependencyInjection;
using Moq;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application;
using Wsa.Gaas.Werewolf.WebApiTests.TDD.ApplicationTest.UseCases;


public class ConfirmPlayerRoleUseCaseTests
{

    [Test]
    [Description("""
            Given: A game started
            When: The player confirms his role
            Then: The player role is returned to the player
                , and the event is broadcasted to all players
            """)]
    public async Task ConfirmPlayerRoleTest1()
    {
        // Given
        var random = new Random();

        // Arrange Ids
        var discordVoiceChannelId = (ulong)random.Next();
        var playerIds = Enumerable.Range(0, 10).Select(x => (ulong)random.Next()).ToArray();
        var playerId = playerIds[random.Next(0, playerIds.Length)];

        // Arrange Game
        var game = new Game(discordVoiceChannelId);
        game.StartGame(playerIds);

        var expectedRole = game.Players.First(x => x.UserId == playerId).Role!.Name;

        // Arrange Repository
        var repository = new Mock<IRepository>();
        repository
            .Setup(x => x.FindByDiscordChannelIdAsync(It.Is<ulong>(x => x == discordVoiceChannelId)))
            .Returns(() => Task.FromResult<Game?>(game))
            ;

        // Arrange Game Event Bus
        var gameEventBus = new Mock<GameEventBus>(
            new Mock<IServiceScopeFactory>().Object
        );
        gameEventBus.Setup(x => x.BroadcastAsync(It.IsAny<PlayerRoleConfirmedEvent>(), It.IsAny<CancellationToken>()));

        // Arrange Use Case
        var useCase = new ConfirmPlayerRoleUseCase(
            repository.Object,
            gameEventBus.Object
        );

        // Arrange Request
        var request = new ConfirmPlayerRoleRequest
        {
            DiscordVoiceChannelId = discordVoiceChannelId,
            PlayerId = playerId,
        };

        // When
        await useCase.ExecuteAsync(request);

    }

    [Test]
    [Description("""
            Given: A Game with a random Discord Voice Channel Id
            When: The Game is not found
            Then: A GameNotFoundException is thrown
            """)]
    public void ConfirmPlayerRoleTest2()
    {
        // Arrange or Given
        var request = new ConfirmPlayerRoleRequest();
        var presenter = new Mock<IPresenter<PlayerRoleConfirmedEvent>>();
        var gameEventBus = new Mock<GameEventBus>(new Mock<IServiceScopeFactory>().Object);

        // Arrange Repository so that it returns null
        var repository = new Mock<IRepository>();
        repository.Setup(r => r.FindByDiscordChannelIdAsync(It.IsAny<ulong>()))
            .Returns(Task.FromResult<Game?>(null));

        var useCase = new ConfirmPlayerRoleUseCase(
            repository.Object,
            gameEventBus.Object
        );

        // Act or When
        // Assert or Then
        Assert.ThrowsAsync(
            typeof(GameNotFoundException),
            async () => await useCase.ExecuteAsync(request, CancellationToken.None)
        );

    }
}
