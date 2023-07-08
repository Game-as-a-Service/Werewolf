namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    internal class GameNotStartedException : GameException
    {
        public GameNotStartedException(ulong discordVoiceChannelId)
            : base($"Game #{discordVoiceChannelId} not started")
        {
        }
    }
}