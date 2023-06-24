using System.Diagnostics;
using Wsa.Gaas.Werewolf.ChatBot.Application.Common;

namespace Wsa.Gaas.Werewolf.DiscordBot.Application.UseCases;

public class GameVersionGetRequest
{
}

public class GameVersionGetUseCase : UseCase<GameVersionGetRequest, string>
{
    public override Task<string> ExecuteAsync(GameVersionGetRequest request, CancellationToken cancellationToken = default)
    {
        var assembly = GetType().Assembly;
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        var version = fileVersionInfo.ProductVersion ?? "Unknown Version";

        return Task.FromResult(version);
    }
}
