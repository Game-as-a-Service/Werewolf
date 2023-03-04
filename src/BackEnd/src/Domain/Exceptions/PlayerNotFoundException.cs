namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(long roomId, long playerId)
            : base($"Game #{roomId}, Player #{playerId} not found")
        {

        }
    }

    public class PlayerRoleNotAssignedException : Exception
    {
        public PlayerRoleNotAssignedException(long playerId)
            : base($"Player #{playerId} role not assigned")
        {

        }
    }

}