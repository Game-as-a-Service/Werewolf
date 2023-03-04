using Wsa.Gaas.Werewolf.Domain.Entities;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public abstract class GameEvent
    {
        protected GameEvent(Game data)
        {
            Data = data;
        }

        public Game Data { get; init; }
        public DateTimeOffset TriggeredOn { get; } = DateTimeOffset.UtcNow;
    }
}