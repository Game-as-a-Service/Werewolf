using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;

public class WerewolfSuicideResponse
{
}

public class WerewolfSuicideEndpoint : WebApiEndpoint<WerewolfSuicideRequest, WerewolfSuicidedEvent, WerewolfSuicideResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordChannelId}/werewolf/suicide");
        AllowAnonymous();
    }

    public async override Task<WerewolfSuicideResponse> ExecuteAsync(WerewolfSuicideRequest req, CancellationToken ct)
    {
        await UseCase.ExecuteAsync(req, this, ct);

        if (ViewModel == null)
        {
            throw new Exception("View Model is null");
        }

        return ViewModel; 
    }

    public override Task PresentAsync(WerewolfSuicidedEvent gameEvent, CancellationToken cancellationToken = default)
    {
        ViewModel = new WerewolfSuicideResponse();

        return Task.CompletedTask;
    }
}
