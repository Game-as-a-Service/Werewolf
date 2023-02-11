using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class PlayerSpeakingEvent : GameEvent
{
    public PlayerSpeakingEvent(Game data) : base(data) { }
}