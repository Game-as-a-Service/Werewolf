using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class GuardianRoundStartedEvent : GameEvent
    {
        public GuardianRoundStartedEvent(Game data) : base(data)
        {
        }
    }
}
