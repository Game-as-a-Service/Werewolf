namespace Wsa.Gaas.Werewolf.Domain.Objects
{
    public class Game
    {
        public Guid Id { get; internal set; }
        public ulong DiscordVoiceChannelId { get; internal set; }
        public bool IsEnded { get; internal set; }

        internal Game()
        {
        }

        public Game(ulong discordVoiceChannelId)
        {
            DiscordVoiceChannelId = discordVoiceChannelId;
        }
    }
}
