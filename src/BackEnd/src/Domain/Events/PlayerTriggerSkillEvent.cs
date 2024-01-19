using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;
public class PlayerTriggerSkillEvent : GameEvent
{
    public PlayerTriggerSkillEvent(Game data) : base(data) { }
}
