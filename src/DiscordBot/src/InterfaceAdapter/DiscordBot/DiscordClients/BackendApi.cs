using System.Linq.Expressions;
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

    public async Task<string> GetGame(ulong discordVoiceChannelId)
    {
        try
        {
            var path = $"/games/{discordVoiceChannelId}";

            var response = await _httpClient.GetAsync(path);

            var request = response.RequestMessage!;

            var rawResponse = 
                $"""
                {request.Method} {request.RequestUri!.PathAndQuery} HTTP/{request.Version}
                {string.Join("\n", request.Headers.Select(x => $"{x.Key}: {string.Join(" ", x.Value)}"))}

                HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}
                {string.Join("\n", response.Headers.Select(x => $"{x.Key}: {string.Join(" ", x.Value)}"))}
                {string.Join("\n", response.Content.Headers.Select(x => $"{x.Key}: {string.Join(" ", x.Value)}"))}

                {await response.Content.ReadAsStringAsync()}
                """;

            return rawResponse;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}