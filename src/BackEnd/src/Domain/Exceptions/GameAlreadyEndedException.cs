namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class GameAlreadyEndedException : GameException
    {
        public GameAlreadyEndedException(ulong discordChannelId) : base($"Game already Ended #{discordChannelId}")
        {
        }
    }
}
