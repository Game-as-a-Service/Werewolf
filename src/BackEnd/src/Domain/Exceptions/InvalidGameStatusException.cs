using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Exceptions;

internal class InvalidGameStatusException : Exception
{
    public InvalidGameStatusException(Game game)
        : base($"Game #{game.RoomId} at Status{game.Status}") { }
}