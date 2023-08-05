using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;


public class WitchUseAntidoteResponse
{
    public required string Message { get; set; }
}

public class WitchUseAntidoteEndpoint : WebApiEndpoint<WitchUseAntidoteRequest, WitchUseAntidoteEvent, WitchUseAntidoteResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{playerId}:useAnitdote");
        AllowAnonymous();
    }

    public override async Task<WitchUseAntidoteResponse> ExecuteAsync(WitchUseAntidoteRequest req, CancellationToken ct)
    {
        await UseCase.ExecuteAsync(req, this, ct);

        if (ViewModel == null)
        {
            throw new Exception("View Model is null");
        }

        // HTTP JSON Response
        //return ViewModel; // <= 把 ViewModel 轉 JSON
        return ViewModel;
    }



    public override Task PresentAsync(WitchUseAntidoteEvent gameEvent, CancellationToken cancellationToken = default)
    {
        ViewModel = new WitchUseAntidoteResponse
        {
            Message = "Ok",
        };

        return Task.CompletedTask;
    }
}


