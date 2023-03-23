namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class PlayerRoleNotAssignedException : Exception
    {
        public PlayerRoleNotAssignedException(ulong playerId)
            : base($"Player #{playerId} role not assigned")
        {

        }
    }

}