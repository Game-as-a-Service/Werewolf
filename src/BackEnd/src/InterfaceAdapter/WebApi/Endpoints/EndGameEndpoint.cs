using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public record EndGameResponse();

    public class EndGameEndpoint : WebApiEndpoint<EndGameRequest, GameEndedEvent, EndGameResponse>
    {
        public override void Configure()
        {
            Post("/games/{DiscordVoiceChannelId}/end");
            AllowAnonymous();
        }

        public override async Task<EndGameResponse> ExecuteAsync(EndGameRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel;
        }

        public override Task PresentAsync(GameEndedEvent gameEvent, CancellationToken cancellationToken = default)
        {
            ViewModel = new EndGameResponse();
            return Task.CompletedTask;
        }

    }
}
