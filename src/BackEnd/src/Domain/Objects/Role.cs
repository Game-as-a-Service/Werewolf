namespace Wsa.Gaas.Werewolf.Domain.Objects
{
    public abstract class Role
    {
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

    public class Werewolf : Role
    {
        public Werewolf()
        {
            Id = 1;
            Name = GetType().Name;
            Faction = Faction.Werewolf;
        }
    }

    public class AlphaWerewolf : Werewolf
    {
        public AlphaWerewolf() : base()
        {
            Id = 2;
        }
    }

    public class Villager : Role
    {
        public Villager()
        {
            Id = 3;
            Name = GetType().Name;
            Faction = Faction.Alliance;
        }
    }

    public abstract class SepcialRole : Role
    {
        public SepcialRole()
        {
            Name = GetType().Name;
            Faction = Faction.Alliance;
        }
    }

    public class Witch : SepcialRole
    {
        public Witch() : base()
        {
            Id = 4;
        }
    }

    public class Seer : SepcialRole
    {
        public Seer() : base()
        {
            Id = 5;
        }
    }

    public class Hunter : SepcialRole
    {
        public Hunter() : base()
        {
            Id = 6;
        }
    }

    public class Guardian : SepcialRole
    {
        public Guardian() : base()
        {
            Id = 7;
        }
    }

}