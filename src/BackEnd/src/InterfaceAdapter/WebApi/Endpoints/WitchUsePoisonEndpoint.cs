using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{

    public class WitchUsePoisonResponse
    {
        public required string Message { get; set; }
    }

    public class WitchUsePoisonEndpoint : WebApiEndpoint<WitchUsePoisonRequest, WitchUsePoisonEvent, WitchUsePoisonResponse>
    {
        public override void Configure()
        {
            Post("/games/{DiscordVoiceChannelId}/players/{playerId}:usePoison");
            AllowAnonymous();
        }

        public override async Task<WitchUsePoisonResponse> ExecuteAsync(WitchUsePoisonRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel; // <= 把 ViewModel 轉 JSON
        }

        public override Task PresentAsync(WitchUsePoisonEvent gameEvent, CancellationToken cancellationToken = default)
        {
            ViewModel = new WitchUsePoisonResponse
            {
                Message = "Ok",
            };

            return Task.CompletedTask;
        }
    }


}
