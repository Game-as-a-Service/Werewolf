namespace Wsa.Gaas.Werewolf.Domain.Exceptions;

using Wsa.Gaas.Werewolf.Domain.Common;

internal class PlayersDuplicatedException : GameException
{
    public PlayersDuplicatedException() : base("Duplicate player id found")
    {
    }
}