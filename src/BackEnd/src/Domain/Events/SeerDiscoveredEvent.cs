using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class SeerDiscoveredEvent : GameEvent
    {
        public SeerDiscoveredEvent(Game data) : base(data)
        {
        }
    }
}
