namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(long discordChannelId) : base($"No active Game found in voice channel #{discordChannelId}.")
        {
        }
    }
}
