using Microsoft.Extensions.DependencyInjection;
using Moq;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.ApplicationTest.UseCaseTests
{

    public class ConfirmPlayerRoleUseCaseTests
    {

        [Test]
        [Description("""
            Given: 
                Game started player role confirmation
            When:
                Player confirms the role
            Then:
                Return player's role
            """)]
        public async Task UseCaseTest()
        {
            // Given
            var random = new Random();

            // Arrange Ids
            var discordVoiceChannelId = (ulong)random.Next();
            var playerIds = Enumerable.Range(0, 10).Select(x => (ulong)x).ToArray();

            // Arrange Game
            var game = new Game(discordVoiceChannelId);
            game.StartGame(playerIds);

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

            // Arrange Presenter
            var presenter = new Mock<IPresenter<PlayerRoleConfirmedEvent>>();
            presenter.Setup(x => x.PresentAsync(It.IsAny<PlayerRoleConfirmedEvent>(), It.IsAny<CancellationToken>()));

            // Arrange Use Case
            var useCase = new ConfirmPlayerRoleUseCase(
                repository.Object,
                gameEventBus.Object
            );

            // When, every player confirms the role
            foreach (var player in game.Players)
            {
                var playerId = player.UserId;
                var expectedRole = player.Role.Name;

                var request = new ConfirmPlayerRoleRequest
                {
                    DiscordVoiceChannelId = discordVoiceChannelId,
                    PlayerId = playerId,
                };

                await useCase.ExecuteAsync(request, presenter.Object);

                // Then
                presenter.Verify(p => p.PresentAsync(
                    It.Is<PlayerRoleConfirmedEvent>(gameEvent =>
                           gameEvent.PlayerId == playerId
                        && gameEvent.Role == expectedRole
                    ),
                    It.IsAny<CancellationToken>()
                ));
            }


        }

        [Test]
        public Task UseCaseTest2()
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

            var gameEventBus = new Mock<GameEventBus>(
                new Mock<IServiceScopeFactory>().Object
            );
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

            return Task.CompletedTask;

        }
    }
}
