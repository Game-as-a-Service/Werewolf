using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class NightfallStartedEvent : GameEvent
    {
        public NightfallStartedEvent(Game data) : base(data)
        {
        }
    }
}