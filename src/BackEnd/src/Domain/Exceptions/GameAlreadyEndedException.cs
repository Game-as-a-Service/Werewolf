namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class GameAlreadyEndedException : Exception
    {
        public GameAlreadyEndedException(ulong discordChannelId) : base($"Game already Ended #{discordChannelId}")
        {
        }
    }
}
