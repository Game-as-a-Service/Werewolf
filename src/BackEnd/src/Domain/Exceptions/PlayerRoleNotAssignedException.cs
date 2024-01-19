using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Domain.Exceptions;
public class PlayerRoleNotAssignedException : GameException
{
    public PlayerRoleNotAssignedException(ulong playerId)
        : base($"Player #{playerId} role not assigned")
    {

    }
}