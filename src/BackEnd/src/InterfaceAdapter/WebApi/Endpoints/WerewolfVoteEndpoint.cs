using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;

/// <summary>
/// Interface Adapter Layer
/// - Controller
/// </summary>

public class WerewolfVoteResponse
{
    public required string Message { get; set; }
}

public class WerewolfVoteEndpoint : WebApiEndpoint<WerewolfVoteRequest, WerewolfVotedEvent, WerewolfVoteResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordChannelId}/werewolf/vote");
        AllowAnonymous();
    }

    public override async Task<WerewolfVoteResponse> ExecuteAsync(WerewolfVoteRequest req, CancellationToken ct)
    {
        await UseCase.ExecuteAsync(req, this, ct);

        if (ViewModel == null)
        {
            throw new Exception("View Model is null");
        }

        return ViewModel;
    }

    public override Task PresentAsync(WerewolfVotedEvent gameEvent, CancellationToken cancellationToken = default)
    {
        ViewModel = new WerewolfVoteResponse
        {
            Message = "Ok",
        };

        return Task.CompletedTask;
    }
}
