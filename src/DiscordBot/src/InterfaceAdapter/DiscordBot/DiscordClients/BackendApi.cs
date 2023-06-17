using System.Net;

namespace Wsa.Gaas.Werewolf.DiscordBot.DiscordClients
{
    public class GameDto
    {
        public string GameId { get; set; }
    }

    internal class BackendApi
    {
        public BackendApi()
        {
        }

        public async Task<GameDto?> CreateGame(ulong discordVoiceChannelId)
        {
            // 呼叫後端 API
            var httpClient = new HttpClient();
            var url = "https://werewolf-api-dev.azurewebsites.net/games";

            var response = await httpClient.PostAsJsonAsync(url, new
            {
                DiscordVoiceChannelId = discordVoiceChannelId,
            });

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var content = await response.Content.ReadAsStringAsync();

                return null;
            }
            
            var gameDto = await response.Content.ReadFromJsonAsync<GameDto>();

            return gameDto!;
        }
    }
}