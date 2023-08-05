namespace Wsa.Gaas.Werewolf.Domain.Exceptions;

internal class GameAlreadyStartedException : GameException
{
    public GameAlreadyStartedException() : base("Game already started")
    {
    }
}