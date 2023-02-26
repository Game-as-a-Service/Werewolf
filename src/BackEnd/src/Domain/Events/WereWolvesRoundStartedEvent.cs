using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class WereWolvesRoundStartedEvent : GameEvent
    {
        public WereWolvesRoundStartedEvent(Game data) : base(data)
        {
        }
    }
}