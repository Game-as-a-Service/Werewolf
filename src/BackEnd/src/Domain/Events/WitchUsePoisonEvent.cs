using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class WitchUsePoisonEvent : GameEvent
    {
        public WitchUsePoisonEvent(Game data) : base(data)
        {
        }
    }
}
