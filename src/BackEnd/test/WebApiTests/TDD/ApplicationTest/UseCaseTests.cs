using Moq;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.ApplicationTest
{

    public class UseCaseTests
    {

        [Test]
        public async Task UseCaseTest()
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
            game.StartPlayerRoleConfirmation();
            var expectedRole = game.Players.First(x => x.UserId == playerId).Role!.Name;

            // Arrange Repository
            var repository = new Mock<IRepository>();
            repository
                .Setup(x => x.FindByDiscordChannelIdAsync(It.Is<ulong>(x => x == discordVoiceChannelId)))
                .Returns(() => Task.FromResult<Game?>(game))
                ;

            // Arrange Game Event Bus
            var gameEventBus = new Mock<GameEventBus>();
            gameEventBus.Setup(x => x.BroadcastAsync(It.IsAny<PlayerRoleConfirmedEvent>(), It.IsAny<CancellationToken>()));

            // Arrange Presenter
            var presenter = new Mock<IPresenter<PlayerRoleConfirmedEvent>>();
            presenter.Setup(x => x.PresentAsync(It.IsAny<PlayerRoleConfirmedEvent>(), It.IsAny<CancellationToken>()));

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
            await useCase.ExecuteAsync(request, presenter.Object);

            // Then
            gameEventBus.Verify(bus => bus.BroadcastAsync(
                It.Is<PlayerRoleConfirmedEvent>(gameEvent => gameEvent.PlayerId == playerId),
                It.IsAny<CancellationToken>()
            ));
            presenter.Verify(p => p.PresentAsync(
                It.Is<PlayerRoleConfirmedEvent>(gameEvent => 
                       gameEvent.PlayerId == playerId 
                    && gameEvent.Role == expectedRole
                ),
                It.IsAny<CancellationToken>()
            ));



        }

        [Test]
        public async Task UseCaseTest2()
        {
            // Arrange or Given
            ulong discordVoiceChannelId = 3;
            ulong playerId = 2;
            var request = new ConfirmPlayerRoleRequest
            {
                DiscordVoiceChannelId = discordVoiceChannelId,
                PlayerId = playerId,
            };
            var presenter = new Mock<IPresenter<PlayerRoleConfirmedEvent>>();
            presenter.Setup(p => p.PresentAsync(It.IsAny<PlayerRoleConfirmedEvent>(), default))
                .Returns(Task.CompletedTask);
                ;

            var cancellationToken = new CancellationToken();
            
            Game? game = null;

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.FindByDiscordChannelIdAsync(It.IsAny<ulong>()))
                .Returns(Task.FromResult(game));

            var gameEventBus = new Mock<GameEventBus>();
            gameEventBus.Setup(x => x.BroadcastAsync(It.IsAny<PlayerRoleConfirmedEvent>(), It.IsAny<CancellationToken>()));


            var useCase = new ConfirmPlayerRoleUseCase(
                repository.Object, 
                gameEventBus.Object
            );

            // Act or When
            //await useCase.ExecuteAsync(request, presenter.Object, cancellationToken);
            

            // Assert or Then

            // ONLY For Exception
            Assert.ThrowsAsync(
                typeof(GameNotFoundException),
                async () => await useCase.ExecuteAsync(request, presenter.Object, cancellationToken)
            );

        }
    }
}
