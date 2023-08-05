namespace Wsa.Gaas.Werewolf.Domain.Exceptions;

public class PlayerNotWerewolfException : GameException
{
    public PlayerNotWerewolfException(string message) : base(message)
    {
    }
}