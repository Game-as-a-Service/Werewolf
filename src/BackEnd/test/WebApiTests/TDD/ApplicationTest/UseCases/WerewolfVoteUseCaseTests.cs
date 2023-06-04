using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.ApplicationTest.UseCases
{
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

            // 1. Give / Arrange
            var faker = new Faker();
            var request = new WerewolfVoteRequest
            {
                DiscordChannelId = faker.Random.ULong(),
                TargetId = faker.Random.ULong(),
                CallerId = faker.Random.ULong(),
            };
            var mockGame = new Mock<Game>(request.DiscordChannelId);
            var gameEvent = new WerewolfVotedEvent(mockGame.Object);
            mockGame
                .Setup(x => x.WerewolfVote(
                    It.Is<ulong>(x => x == request.CallerId),
                    It.Is<ulong>(x => x == request.TargetId)
                ))
                .Returns(gameEvent)
                ;

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
                mockPresenter.Object,
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

            // 驗證 Use Case 有呼叫 Presenter 的【推】
            mockPresenter.Verify(x =>
                x.PresentAsync(
                    It.Is<WerewolfVotedEvent>(x => x == gameEvent),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once()
            );

            // 如果設置好一個測試，然後沒過，我們要修改程式碼的哪個部分??
            // 我們應該修改，單元測試的【單元】
            // 這邊的目標【單元】 是 Use Case
            // 預計:
            //   設置好這個測試，然後沒過，我們要修改程式碼的 Use Case (WerewolfVoteUseCase)

            // Oliver: 我們可以驗證存的結果，有沒有跟我們預期的一樣 
            // 因為【驗證存的結果】，如果沒有跟我們預期的一樣，我們會需要去改 Domain or Repository
            // 所以不是我們要的。

        }
    }
}
