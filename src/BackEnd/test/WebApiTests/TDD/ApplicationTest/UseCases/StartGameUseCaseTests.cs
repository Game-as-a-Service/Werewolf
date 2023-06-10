using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq.Expressions;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApiTests.TDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.ApplicationTest.UseCases
{
    internal class StartGameUseCaseTests
    {
        [Test]
        [Description("""
            Given: Game Started
            When: Starting game with 12 Players
            Then: Error, game already started
            """)]
        public async Task StartGameUseCaseTest小西()
        {

            // declare Faker
            var faker = new Faker();

            //Given
            var request = new StartGameRequest()
            {
                DiscordVoiceChannelId = faker.Random.ULong(),
                Players = new ulong[]
                {
                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),

                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),

                    faker.Random.ULong(),
                    faker.Random.ULong(),
                }
            };
            // Arrange Game
            var mockGame = new Mock<Game>(request.DiscordVoiceChannelId);

            // Arrange Repository
            var repository = new Mock<IRepository>();
            repository
                .Setup(x => x.FindByDiscordChannelIdAsync(It.Is<ulong>(x => x == request.DiscordVoiceChannelId)))
                .Returns(() => Task.FromResult<Game?>(mockGame.Object))
                ;
            // Arrange Game Event Bus
            var eventBus = new Mock<GameEventBus>(new Mock<IServiceScopeFactory>().Object);

            // Arrange Presenter
            var presenter = new Mock<IPresenter<GameStartedEvent>>();
            presenter.Setup(x => x.PresentAsync(It.IsAny<GameStartedEvent>(), It.IsAny<CancellationToken>()));

            // Arrange Use Case
            var useCase = new StartGameUseCase(repository.Object, eventBus.Object);

            //When
            await useCase.ExecuteAsync(request, presenter.Object);

            //Then：查改存推
            // 驗證 Use Case 有呼叫 Repository 的【查】
            repository.Verify(x =>
                x.FindByDiscordChannelIdAsync(It.Is<ulong>(x => x == request.DiscordVoiceChannelId)),
                Times.Once()
            );

            //驗證 Use Case 有呼叫 Game 的【改】
            mockGame.Verify(x => x.StartGame(It.IsAny<ulong[]>()), Times.Once());

            presenter.Verify(p => p.PresentAsync(
               It.Is<GameStartedEvent>(gameStartedEvent =>
                      gameStartedEvent.Data.Players.Count == 12),
               It.IsAny<CancellationToken>()
           ), Times.Once());

            // 驗證 Use Case 有呼叫 Game 的【存】
            repository.Verify(x =>
                x.SaveAsync(It.Is<Game>(x => x == mockGame.Object)),
                Times.Once()
            );

            // 驗證 Use Case 有呼叫 Presenter 的【推】
            eventBus.Verify(bus => bus.BroadcastAsync(
                It.Is<GameStartedEvent>(gameStartedEvent =>
                gameStartedEvent.Data.Players.Count == 12),
                It.IsAny<CancellationToken>()
                ), Times.Once());

            presenter.Verify(p => p.PresentAsync(
                It.Is<GameStartedEvent>(gameStartedEvent =>
                       gameStartedEvent.Data.Players.Count == 12),
                It.IsAny<CancellationToken>()
            ), Times.Once());
        }

        [Test]
        [Description("""
            Given: Game Started
            When: Starting game with 12 Players
            Then: Error, game already started
            """)]
        public async Task StartGameUseCaseTest小軟()
        {
            // 1. Given
            var faker = new Faker();

            var request = new StartGameRequest()
            {
                DiscordVoiceChannelId = faker.Random.ULong(),
                Players = new ulong[]
                {
                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),

                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),
                    faker.Random.ULong(),

                    faker.Random.ULong(),
                    faker.Random.ULong()
                }
            };

            var mockGame = new Mock<Game>(request.DiscordVoiceChannelId);

            var mockRepository = new Mock<IRepository>();
            mockRepository
                .Setup(x => x.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId))
                .Returns(() => Task.FromResult<Game?>(mockGame.Object));
            mockRepository
                .Setup(x => x.Save(mockGame.Object));

            var gameEvent = new GameStartedEvent(mockGame.Object);

            var mockEventBus = new Mock<GameEventBus>(
                new Mock<IServiceScopeFactory>().Object
            );

            var mockPresenter = new Mock<IPresenter<GameStartedEvent>>();

            var useCase = new StartGameUseCase(
                mockRepository.Object,
                mockEventBus.Object
            );

            // 2. When
            await useCase.ExecuteAsync(
               request,
               mockPresenter.Object,
               CancellationToken.None
            );

            // 3. Then

            // 驗證 Use Case 有呼叫 Game 的【改】
            mockGame.Verify(
                x => x.StartGame(request.Players),
                Times.Once());

            // 驗證 Use Case 有呼叫 Game 的【存】
            mockRepository.Verify(x =>
                x.Save(It.Is<Game>(x => x == mockGame.Object)),
                Times.Once()
            );

            // 驗證 Use Case 有呼叫 GameEventBus 的【推】SignalR
            mockEventBus.Verify(bus => bus.BroadcastAsync(
               It.Is<GameStartedEvent>(x => x.Data.Players.Count == 12),
               It.IsAny<CancellationToken>()
               ), Times.Once());

            // 驗證 Use Case 有呼叫 Presenter 的【推】PresentAsync
            mockPresenter.Verify(x =>
                x.PresentAsync(
                    It.Is<GameStartedEvent>(x => x == gameEvent),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once()
            );
        }



        [Test]
        [Description("""
            Given: Game Started
            When: Starting game with 12 Players
            Then: Error, game already started
            """)]
        public async Task StartGameUseCaseTestPuTao()
        {
            //Given
            // 在 Given 階段，我們需要設置甚麼?
            // request
            var faker = new Faker();
            var discordVoiceChannelId = faker.Random.ULong();
            var players = Enumerable.Range(0, 12).Select(x => faker.Random.ULong()).ToArray();

            var request = new StartGameRequest()
            {
                DiscordVoiceChannelId = discordVoiceChannelId,
                Players = players,
            };


            // 接下來設置 UseCase <= 目標
            // repository, GameEventBus

            var mockGame = new Mock<Game>(discordVoiceChannelId);
            var mockRepository = new Mock<IRepository>();
            mockRepository
                .Setup(x => x.FindByDiscordChannelIdAsync(discordVoiceChannelId))
                .ReturnsAsync(mockGame.Object);

            var mockEventBus = new Mock<GameEventBus>(
                new Mock<IServiceScopeFactory>().Object
            );

            var gameEvent = new GameStartedEvent(mockGame.Object);
            var mockPresenter = new Mock<IPresenter<GameStartedEvent>>();

            // Mock Game Event

            var useCase = new StartGameUseCase(mockRepository.Object, mockEventBus.Object);


            //When
            await useCase.ExecuteAsync(request, mockPresenter.Object);


            //Then
            // 查
            mockRepository.Verify(g => g.FindByDiscordChannelIdAsync(It.Is<ulong>(x => x == request.DiscordVoiceChannelId)), Times.Once);
            // 改
            mockGame.Verify(g => g.StartGame(It.Is<ulong[]>(x => x.Count() == players.Count())), Times.Once);
            //存
            mockRepository.Verify(v => v.Save(It.Is<Game>(x => x == mockGame.Object)), Times.Once);
            //推
            Expression<Func<GameStartedEvent, bool>> checkFunc = x =>
                x.Data.DiscordVoiceChannelId.ToString() == discordVoiceChannelId.ToString() &&
                x.Data.Players.Count == 12 &&
                x.Data.Status == GameStatus.Started;

            mockEventBus.Verify(v => v.BroadcastAsync(It.Is(checkFunc), It.IsAny<CancellationToken>()), Times.Once);
            mockPresenter.Verify(v => v.PresentAsync(It.Is(checkFunc), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Test]
        [Description("""
            Given: Game Started
            When: Starting game with 12 Players
            Then: Error, game already started
            """)]
        public async Task StartGameUseCaseTestYoyo()
        {

            // 1. Given / Arrange
            var faker = new Faker();

            //Given
            var players = new ulong[12];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = faker.Random.ULong();
            }


            var request = new StartGameRequest()
            {
                DiscordVoiceChannelId = faker.Random.ULong(),
                Players = players
            };

            var req = new Faker<StartGameRequest>()
                .RuleFor(x => x.DiscordVoiceChannelId, f => f.Random.ULong());


            //When

            //Then(驗證)：確認遊戲已設置+有玩家12位

            return Task.CompletedTask;
        }

        [Test]
        [Description("""
            Given: Game Started
            When: Starting game with 12 Players
            Then: Error, game already started
            """)]
        public async Task StartGameUseCaseTestRick()
        {
            var faker = new Faker();

            // Arrange
            var discordVoiceChannelId = faker.Random.ULong();
            var players = Enumerable.Range(0, 12)
                .Select(x => faker.Random.ULong())
                .ToArray()
                ;

            var request = new StartGameRequest()
            {
                DiscordVoiceChannelId = discordVoiceChannelId,
                Players = players,
            };

            var game = new Mock<Game>(request.DiscordVoiceChannelId);
            var gameEvent = new GameStartedEvent(game.Object);
            game
                .Setup(x => x.StartGame(It.IsAny<ulong[]>()))
                .Returns(gameEvent);

            var repository = new Mock<IRepository>();
            repository
                .Setup(x => x.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId))
                .ReturnsAsync(game.Object);
            repository
                .Setup(x => x.Save(It.Is<Game>(x => x == game.Object)));

            var presenter = new Mock<IPresenter<GameStartedEvent>>();


            var gameEventBus = new Mock<GameEventBus>();

            var useCase = new StartGameUseCase(
                repository.Object,
                gameEventBus.Object
            );

            // 
            await useCase.ExecuteAsync(request, presenter.Object);

            // Assert
            // 驗證 Use Case 有呼叫 Repository 的【查】
            repository.Verify(
                x => x.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId),
                Times.Once()
            );

            // 驗證 Use Case 有呼叫 Game 的【改】
            game.Verify(
                x => x.StartGame(It.IsAny<ulong[]>()),
                Times.Once()
            );

            // 驗證 Use Case 有呼叫 Repository 的【存】
            repository.Verify(
                x => x.Save(It.Is<Game>(x => x == game.Object)),
                Times.Once()
            );

            // 驗證 Use Case 有呼叫 EventBus 的【推】
            gameEventBus.Verify(
                x => x.BroadcastAsync(
                    It.IsAny<GameStartedEvent>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once()
            );

            // 驗證 Use Case 有呼叫 Presenter 的【推】
            presenter.Verify(
                x => x.PresentAsync(
                    It.IsAny<GameStartedEvent>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once()
            );



        }
    }
}
