namespace Wsa.Gaas.Werewolf.Domain.Objects.Roles;
public class Werewolf : Role
{
    public Werewolf()
    {
        Id = 1;
        Name = GetType().Name;
        Faction = Faction.Werewolf;
    }
}