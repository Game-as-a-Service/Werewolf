using System.Net;
using Wsa.Gaas.Werewolf.DiscordBot.Dtos;

namespace Wsa.Gaas.Werewolf.DiscordBot.DiscordClients;


internal class BackendApi
{
    private readonly HttpClient _httpClient;
    public BackendApi()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://werewolf-api-dev.azurewebsites.net")
        };
    }

    public async Task<GameDto?> CreateGame(ulong discordVoiceChannelId)
    {
        // 呼叫後端 API
        var path = "/games";

        var response = await _httpClient.PostAsJsonAsync(path, new
        {
            DiscordVoiceChannelId = discordVoiceChannelId,
        });

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            return null;
        }
        
        var gameDto = await response.Content.ReadFromJsonAsync<GameDto>();

        return gameDto!;
    }

    public async Task<GameDto?> GetGame(ulong discordVoiceChannelId)
    {
        var path = $"/games/{discordVoiceChannelId}";

        var gameDto = await _httpClient.GetFromJsonAsync<GameDto>(path);

        return gameDto;
    }
}