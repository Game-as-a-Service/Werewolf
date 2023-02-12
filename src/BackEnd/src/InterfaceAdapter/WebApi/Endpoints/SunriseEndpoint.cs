using Application.UseCases;
using Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public record SunriseResponse(
        string GameId,
        PlayerVm[] Players
    );

    public class SunriseEndpoint : WebApiEndpoint<SunriseRequest, SunriseEvent, SunriseResponse>
    {
        public SunriseEndpoint()
        {
        }

        public override void Configure()
        {
            Post("/games/{DiscordVoiceChannelId}/sunrise");
            AllowAnonymous();   
        }

        public override async Task<SunriseResponse> ExecuteAsync(SunriseRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel;
        }

        public override Task PresentAsync(SunriseEvent gameEvent, CancellationToken cancellationToken = default)
        {
            ViewModel = new SunriseResponse
            (
                gameEvent.Data.DiscordVoiceChannelId.ToString(),
                gameEvent.Data.Players.Select(PlayerVm.FromDomain).ToArray()
            );

            return Task.CompletedTask;
        }
    }
}