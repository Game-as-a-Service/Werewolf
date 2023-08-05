using Wsa.Gaas.Werewolf.Domain.Objects.Roles;

namespace Wsa.Gaas.Werewolf.Domain.Objects;

public abstract class Role
{
    public static readonly Role
          WEREWOLF = new Roles.Werewolf()
        , ALPHAWEREWOLF = new AlphaWerewolf()
        , VILLAGER = new Villager()
        , WITCH = new Witch()
        , SEER = new Seer()
        , HUNTER = new Hunter()
        , GUARDIAN = new Guardian()
        ;

    public int Id { get; internal set; }
    public string Name { get; internal set; } = string.Empty;
    public Faction Faction { get; internal set; }
}

public enum Faction
{
    None,
    Alliance,
    Werewolf,
}

public abstract class SepcialRole : Role
{
    public SepcialRole()
    {
        Name = GetType().Name;
        Faction = Faction.Alliance;
    }
}