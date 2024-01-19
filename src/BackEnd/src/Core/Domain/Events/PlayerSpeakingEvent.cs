using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class PlayerSpeakingEvent : GameEvent
{
    public PlayerSpeakingEvent(Game data) : base(data) { }
}