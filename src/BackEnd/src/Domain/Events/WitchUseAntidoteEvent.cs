using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class WitchUseAntidoteEvent : GameEvent
    {
        public WitchUseAntidoteEvent(Game data) : base(data)
        {
        }
    }
}
