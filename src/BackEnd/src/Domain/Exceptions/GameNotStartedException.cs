using System.Runtime.Serialization;

namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    internal class GameNotStartedException : Exception
    {
        public GameNotStartedException(long roomId)
            : base($"Game #{roomId} not started")
        {
        }
    }
}