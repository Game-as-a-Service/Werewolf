using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public record CreateGameResponse(
        Guid GameId,
        ulong DiscordVoiceChannelId
    );

    public class CreateGamePresenter : IPresenter<GameCreatedEvent>
    {
        public CreateGameResponse? ViewModel { get; set; }

        public Task PresentAsync(GameCreatedEvent saidEvent)
        {
            ViewModel = new CreateGameResponse(
                saidEvent.GameId,
                saidEvent.DiscordVoiceChannelId
            );

            return Task.CompletedTask;
        }
    }

    public class CreateGameEndpoint : WebApiEndpoint<CreateGameRequest, GameCreatedEvent, CreateGameResponse>
    {
        public CreateGamePresenter Presenter { get; } = new CreateGamePresenter();

        public override void Configure()
        {
            Post("/games");
            AllowAnonymous();
        }

        public override async Task<CreateGameResponse> ExecuteAsync(CreateGameRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, Presenter);

            if (Presenter.ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return Presenter.ViewModel;
        }
    }
}
