namespace Wsa.Gaas.Werewolf.Application.UseCases;
public class WitchUsePoisonRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
    public ulong PlayerId { get; set; }
    public ulong TargetPlayerId { get; set; }
}

public class WitchUsePoisonResponse
{
    public required string Message { get; set; }
}
public class WitchUsePoisonUseCase : UseCase<WitchUsePoisonRequest, WitchUsePoisonResponse>
{
    public WitchUsePoisonUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public override async Task<WitchUsePoisonResponse> ExecuteAsync(WitchUsePoisonRequest request, CancellationToken cancellationToken = default)
    {
        // 查
        var game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);

        if (game == null)
        {
            throw new GameNotFoundException(request.DiscordVoiceChannelId);
        }

        // 改
        var events = game.WitchUsePoison(request.PlayerId, request.TargetPlayerId);

        // 存
        await Repository.SaveAsync(game);

        // 推
        return new WitchUsePoisonResponse() { Message = "Ok" };
    }
}

