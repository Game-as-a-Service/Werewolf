using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public class WerewolfSuicidedEvent : GameEvent
    {
        public WerewolfSuicidedEvent(Game data) : base(data)
        {
        }
    }
}