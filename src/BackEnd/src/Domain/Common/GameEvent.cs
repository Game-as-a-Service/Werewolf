namespace Wsa.Gaas.Werewolf.Domain.Common
{
    public class GameEvent
    {
        public Guid GameId { get; set; }
        public ulong DiscordVoiceChannelId { get; set; }
    }
}
