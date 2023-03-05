using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;
using Wsa.Gaas.Werewolf.WebApi.Endpoints.Response;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public class CreateGameEndpoint : WebApiEndpoint<CreateGameRequest, GameCreatedEvent, CreateGameResponse>
    {
        public override void Configure()
        {
            Post("/games");
            AllowAnonymous();
        }

        public override async Task<CreateGameResponse> ExecuteAsync(CreateGameRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel;
        }

        public override Task PresentAsync(GameCreatedEvent saidEvent, CancellationToken cancellationToken = default)
        {
            ViewModel = new CreateGameResponse(
                saidEvent.Data.RoomId.ToString()
            );

            return Task.CompletedTask;
        }
    }
}
