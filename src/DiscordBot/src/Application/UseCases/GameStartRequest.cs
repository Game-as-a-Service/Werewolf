using Wsa.Gaas.Werewolf.ChatBot.Application.Common;

namespace Wsa.Gaas.Werewolf.DiscordBot.Application.UseCases;

public class GameStartRequest
{
    public required ulong[] Players { get; set; }
}

public class GameStartUseCase : UseCase<GameStartRequest, string>
{
    public override Task<string> ExecuteAsync(GameStartRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
