namespace Wsa.Gaas.Werewolf.Domain.Exceptions;
public class GameNotFoundException : GameException
{
    public GameNotFoundException(ulong discordChannelId)
        : base($"No active Game found in voice channel #{discordChannelId}.")
    {
    }
}
