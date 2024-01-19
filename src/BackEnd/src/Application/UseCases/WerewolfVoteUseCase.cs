using Wsa.Gaas.Werewolf.Application.Common;

namespace Wsa.Gaas.Werewolf.Application.UseCases;
public class WerewolfVoteRequest
{
    public ulong DiscordChannelId { get; set; }
    public ulong CallerId { get; set; }
    public ulong TargetId { get; set; }
}

public class WerewolfVoteResponse
{
    public required string Message { get; set; }
}

public class WerewolfVoteUseCase : UseCase<WerewolfVoteRequest, WerewolfVoteResponse>
{
    public WerewolfVoteUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public override async Task<WerewolfVoteResponse> ExecuteAsync(WerewolfVoteRequest request, CancellationToken cancellationToken = default)
    {
        // 查
        var game = await Repository.FindByDiscordChannelIdAsync(request.DiscordChannelId) ?? throw new GameNotFoundException(request.DiscordChannelId);

        // 改 => Vote
        var events = game.WerewolfVote(request.CallerId, request.TargetId);

        // 存
        await Repository.SaveAsync(game);

        // 推 => SignalR
        //await GameEventBus.BroadcastAsync(events, cancellationToken);

        // 推 => Restful API
        return new WerewolfVoteResponse
        {
            Message = "Ok",
        };

    }
}
