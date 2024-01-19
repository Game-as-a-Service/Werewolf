namespace Wsa.Gaas.Werewolf.Domain.Objects.Roles;
public class Villager : Role
{
    public Villager()
    {
        Id = 3;
        Name = GetType().Name;
        Faction = Faction.Alliance;
    }
}