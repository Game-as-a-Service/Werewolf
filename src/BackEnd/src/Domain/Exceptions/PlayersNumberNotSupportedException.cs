namespace Wsa.Gaas.Werewolf.Domain.Exceptions;


internal class PlayersNumberNotSupportedException : GameException
{
    public PlayersNumberNotSupportedException()
        : base("Number of players must be between 9 and 12")
    {
    }
}