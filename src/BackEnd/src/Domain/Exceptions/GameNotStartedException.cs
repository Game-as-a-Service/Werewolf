using System.Runtime.Serialization;

namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    internal class GameNotStartedException : Exception
    {
        public GameNotStartedException(ulong discordVoiceChannelId)
            : base($"Game #{discordVoiceChannelId} not started")
        {
        }
    }
}