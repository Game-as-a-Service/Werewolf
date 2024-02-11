using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Application.UseCases.Players;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.ApplicationTest.UseCases;
public class WerewolfVoteUseCaseTests
{
    [Test]
    [Description("""
            Given:
            有ABC為狼人
            DEFGHI六個個玩家存活

            When:
            狼人們 ABC 都投票 B 玩家

            Then:
            B 玩家得 3 票
            ACDEFGHI 八個玩家得 0 票
            """)]
    public async Task WerewolfVoteUseCaseTest()
    {
        // 目標: 驗證 Use Case 有做到【查改存推】

        // 1. Given / Arrange
        var faker = new Faker();

        // HTTP Request
        var request = new WerewolfVoteRequest
        {
            DiscordChannelId = faker.Random.ULong(),
            TargetId = faker.Random.ULong(),
            CallerId = faker.Random.ULong(),
        };

        // Mock Game
        var mockGame = new Mock<Game>(request.DiscordChannelId);

        // Mock Game Event
        var gameEvent = new WerewolfVotedEvent(mockGame.Object);

        // 設置 mockGame 行為的條件
        mockGame
            .Setup(x => x.WerewolfVote(
                // 當 callerId 等於 request.CallerId
                It.Is<ulong>(x => x == request.CallerId),

                // 當 targetId 等於 request.TargetId
                It.Is<ulong>(x => x == request.TargetId)
            ))
            .Returns(gameEvent)
            ;

        // game.WerewolfVote(1, 3); => gameEvent
        // game.WerewolfVote(2, 4); => exception

        // Mock Repository
        var mockRepository = new Mock<IRepository>();
        mockRepository
            .Setup(x => x.FindByDiscordChannelIdAsync(It.Is<ulong>(x => x == request.DiscordChannelId)))
            .Returns(() => Task.FromResult<Game?>(mockGame.Object))
            ;

        var mockEventBus = new Mock<GameEventBus>(
            new Mock<IServiceScopeFactory>().Object
        );
        var mockPresenter = new Mock<IPresenter<WerewolfVotedEvent>>();

        var useCase = new WerewolfVoteUseCase(
            mockRepository.Object,
            mockEventBus.Object
        );

        // 2. When / Act
        await useCase.ExecuteAsync(
            request,
            CancellationToken.None
        );

        // 3. Then / Assert
        // 目標: 驗證 Use Case 有做到【查改存推】

        // 驗證 Use Case 有呼叫 Repository 的【查】
        mockRepository.Verify(x =>
            x.FindByDiscordChannelIdAsync(It.Is<ulong>(x => x == request.DiscordChannelId)),
            Times.Once()
        );

        // 驗證 Use Case 有呼叫 Game 的【改】
        mockGame.Verify(
            x => x.WerewolfVote(
                It.Is<ulong>(x => x == request.CallerId),
                It.Is<ulong>(x => x == request.TargetId)
            ),
            Times.Once()
        );

        // 驗證 Use Case 有呼叫 Game 的【存】
        mockRepository.Verify(x =>
            x.SaveAsync(It.Is<Game>(x => x == mockGame.Object)),
            Times.Once()
        );
    }
}
