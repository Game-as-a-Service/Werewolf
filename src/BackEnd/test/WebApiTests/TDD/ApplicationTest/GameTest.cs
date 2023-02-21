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

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.ApplicationTest
{

    public class GameTest
    {

        [Test]
        public async Task UseCaseTest()
        {
            // Given
            var repository = new Mock<IRepository>();
            var gameEventBus = new Mock<GameEventBus>();
            var presenter = new Mock<IPresenter<PlayerRoleConfirmedEvent>>();

           
            var useCase = new ConfirmPlayerRoleUseCase(
                repository.Object,
                gameEventBus.Object
            );

            var request = new ConfirmPlayerRoleRequest
            {
                DiscordVoiceChannelId = 1,
                PlayerId = 3,
            };

            // When
            await useCase.ExecuteAsync(request, presenter.Object);

            // Then
            repository.Verify(r => r.Save(It.Is<Game>(g => g.DiscordVoiceChannelId == 1)));
            gameEventBus.Verify(bus => bus.BroadcastAsync(
                It.Is<PlayerRoleConfirmedEvent>(gameEvent => gameEvent.PlayerId == 3),
                It.IsAny<CancellationToken>()
            ));
            presenter.Verify(p => p.PresentAsync(
                It.Is<PlayerRoleConfirmedEvent>(gameEvent => gameEvent.PlayerId == 3 && gameEvent.Role != string.Empty),
                It.IsAny<CancellationToken>()
            ));



        }
    }
}
