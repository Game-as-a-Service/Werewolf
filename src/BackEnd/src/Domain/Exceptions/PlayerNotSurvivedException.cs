using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Domain.Exceptions;
public class PlayerNotSurvivedException : GameException
{
    public PlayerNotSurvivedException(int playerNumber)
        : base($"Player {playerNumber} not survived")
    {
    }
}
