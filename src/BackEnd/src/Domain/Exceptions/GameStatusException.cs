using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    internal class GameStatusException : Exception
    {
        public GameStatusException(GameStatus expected, GameStatus actual)
            : base($"Expect Game Status {expected}, but got {actual}")
        {
        }

    }
}