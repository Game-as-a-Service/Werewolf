namespace Wsa.Gaas.Werewolf.DiscordBot.Options
{
    public class DiscordBotOptions
    {
        public string DiscordOAuthUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public string Permissions { get; set; } = string.Empty;
    }
}
