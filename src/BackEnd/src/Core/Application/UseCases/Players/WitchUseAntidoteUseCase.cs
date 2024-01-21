namespace Wsa.Gaas.Werewolf.Application.UseCases.Players;
public class WitchUseAntidoteRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
    public ulong PlayerId { get; set; }
}

public class WitchUseAntidoteResponse
{
    public required string Message { get; set; }
}


public class WitchUseAntidoteUseCase : UseCase<WitchUseAntidoteRequest, WitchUseAntidoteResponse>
{
    public WitchUseAntidoteUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public override async Task<WitchUseAntidoteResponse> ExecuteAsync(WitchUseAntidoteRequest request, CancellationToken cancellationToken = default)
    {
        // 查
        var game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);

        if (game == null)
        {
            throw new GameNotFoundException(request.DiscordVoiceChannelId);
        }

        // 改
        var events = game.WitchUseAntidote(request.PlayerId);

        // 存
        await Repository.SaveAsync(game);

        // 推
        return new WitchUseAntidoteResponse { Message = "Ok" };
    }
}
