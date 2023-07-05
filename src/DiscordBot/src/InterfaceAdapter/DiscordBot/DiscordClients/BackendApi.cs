using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wsa.Gaas.Werewolf.DiscordBot.Dtos;

namespace Wsa.Gaas.Werewolf.DiscordBot.DiscordClients;


public class BackendApi
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public BackendApi()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://werewolf-api-dev.azurewebsites.net")
            //BaseAddress = new Uri("https://localhost:7009")
        };
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new JsonStringEnumConverter());
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

         var gameDto = await _httpClient.GetFromJsonAsync<GameDto>(path, _options);

        return gameDto;
    }

    public async Task<GameDto> StartGame(ulong discordVoiceChannelId, List<ulong> players)
    {
        var path = $"/games/{discordVoiceChannelId}/start";

        var responseMessage = await _httpClient.PostAsJsonAsync(path, new
        {
            players
        });

        responseMessage.EnsureSuccessStatusCode();

        var gameDto = await responseMessage.Content.ReadFromJsonAsync<GameDto>();

        return gameDto!;
    }
}