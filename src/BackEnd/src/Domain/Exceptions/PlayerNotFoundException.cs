namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(ulong discordVoiceChannelId, ulong playerId)
            : base($"Game #{discordVoiceChannelId}, Player #{playerId} not found")
        {

        }
    }

    public class PlayerRoleNotAssignedException : Exception
    {
        public PlayerRoleNotAssignedException(ulong playerId)
            : base($"Player #{playerId} role not assigned")
        {

        }
    }

}