namespace Wsa.Gaas.Werewolf.DiscordBot.ViewModels
{
    public class PlayerVm
    {
        public string Id { get; set; } = string.Empty;
        public int PlayerNumber { get; set; }
        public bool IsDead { get; set; }
        public string? Role { get; set; }
    }
}