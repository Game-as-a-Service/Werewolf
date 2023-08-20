using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;

public class StartGameEndpoint : WebApiEndpoint<StartGameRequest, PlayerRoleConfirmationStartedEvent, GetGameResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/start");
        AllowAnonymous();
    }

    public override async Task<GetGameResponse> ExecuteAsync(StartGameRequest req, CancellationToken ct)
    {
        await UseCase.ExecuteAsync(req, this, ct);

        if (ViewModel == null)
        {
            throw new Exception("View Model is null");
        }

        // HTTP JSON Response
        return ViewModel; // <= 把 ViewModel 轉 JSON
    }

    public override Task PresentAsync(PlayerRoleConfirmationStartedEvent gameEvent, CancellationToken cancellationToken = default)
    {
        ViewModel = new GetGameResponse(gameEvent);

        return Task.CompletedTask;
    }
}
