namespace Wsa.Gaas.Werewolf.DiscordBot.ViewModels
{
    public class GameVm
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<PlayerVm> Players { get; set; } = new List<PlayerVm>();
        public ulong? CurrentSpeakingPlayer { get; set; }
    }
}
