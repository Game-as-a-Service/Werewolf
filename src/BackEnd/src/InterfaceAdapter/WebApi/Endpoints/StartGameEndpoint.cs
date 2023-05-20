using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public record StartGameResponse(
        string GameId,
        PlayerVm[] Players
    );

    public class StartGameEndpoint : WebApiEndpoint<StartGameRequest, GameStartedEvent, StartGameResponse>
    {
        public override void Configure()
        {
            Post("/games/{DiscordVoiceChannelId}/start");
            AllowAnonymous();
        }

        public override async Task<StartGameResponse> ExecuteAsync(StartGameRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            // HTTP JSON Response
            return ViewModel; // <= 把 ViewModel 轉 JSON
        }

        public override Task PresentAsync(GameStartedEvent gameEvent, CancellationToken cancellationToken = default)
        {
            ViewModel = new StartGameResponse
            (
                gameEvent.Data.DiscordVoiceChannelId.ToString(),
                gameEvent.Data.Players.Select(PlayerVm.FromDomain).ToArray()
            );
            return Task.CompletedTask;
        }
    }

}
