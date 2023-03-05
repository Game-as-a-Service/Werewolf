using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;
using Wsa.Gaas.Werewolf.WebApi.Endpoints.Response;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public class StartGameEndpoint : WebApiEndpoint<StartGameRequest, GameStartedEvent, StartGameResponse>
    {
        public override void Configure()
        {
            Post("/games/{RoomId}/start");
            AllowAnonymous();
        }

        public override async Task<StartGameResponse> ExecuteAsync(StartGameRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel;
        }

        public override Task PresentAsync(GameStartedEvent gameEvent, CancellationToken cancellationToken = default)
        {
            ViewModel = new StartGameResponse();

            return Task.CompletedTask;
        }
    }
}