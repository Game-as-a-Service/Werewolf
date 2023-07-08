namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class PlayerNotFoundException : GameException
    {
        public PlayerNotFoundException(ulong discordVoiceChannelId, ulong playerId)
            : base($"Game #{discordVoiceChannelId}, Player #{playerId} not found")
        {

        }
    }

}