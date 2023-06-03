using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ulong discordVoiceChannelId = 1;
            var playerIds = new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var game = new Game(discordVoiceChannelId);
            game.StartGame(playerIds);

            var repository = new Mock<IRepository>();
            repository
                .Setup(x => x.FindByDiscordChannelIdAsync(It.IsAny<ulong>()))
                .Returns(() => Task.FromResult<Game?>(game))
                ;

            var eventBus = new Mock<GameEventBus>();
            var presenter = new Mock<IPresenter<WerewolfVotedEvent>>();

            var useCase = new WerewolfVoteUseCase(
                repository.Object,
                eventBus.Object
            );

            var request = new WerewolfVoteRequest
            {
            };

            // 2. When / Act
            await useCase.ExecuteAsync(
                request,
                presenter.Object
            );

            // 3. Then / Assert
            // 目標: 驗證 Use Case 有做到【查改存推】

            // 驗證

            // Rick 需要想想怎麼驗證，下次準備完整一點再來分享

            // 如果設置好一個測試，然後沒過，我們要修改程式碼的哪個部分??
            // 我們應該修改，單元測試的【單元】
            // 【單元】 => Use Case
            // 如果設置好這個測試，然後沒過，我們要修改程式碼的 Use Case (WerewolfVoteUseCase)

            // Oliver: 我們可以驗證存的結果，有沒有跟我們預期的一樣 (No，不是我們要的)
            // 驗證存的結果，沒有跟我們預期的一樣，我們要去改 Domain / Repository

        }
    }
}
