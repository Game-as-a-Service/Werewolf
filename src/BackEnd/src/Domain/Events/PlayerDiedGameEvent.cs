using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Common;

public enum SkillTrigger
{
    None,
    Shot,
}

public class PlayerDiedGameEvent : GameEvent
{
    // 讓前端知道，技能觸發
    public SkillTrigger Skill { get; set; }

    public PlayerDiedGameEvent(Game data) : base(data)
    {
    }
}
