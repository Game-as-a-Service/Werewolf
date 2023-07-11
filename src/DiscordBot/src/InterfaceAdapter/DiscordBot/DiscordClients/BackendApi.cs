using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wsa.Gaas.Werewolf.DiscordBot.Dtos;
using Wsa.Gaas.Werewolf.DiscordBot.Options;

namespace Wsa.Gaas.Werewolf.DiscordBot.DiscordClients;

public class BackendApi
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly BackendApiEndpointOptions _apiOptions;

    public BackendApi(IOptions<BackendApiEndpointOptions> options)
    {
        _apiOptions = options.Value;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_apiOptions.Endpoint),
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<GameDto?> CreateGame(ulong discordVoiceChannelId)
    {
        // 呼叫後端 API
        var path = "/games";

        var response = await _httpClient.PostAsJsonAsync(path, new
        {
            DiscordVoiceChannelId = discordVoiceChannelId,
        });

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return null;
        }

        var gameDto = await response.Content.ReadFromJsonAsync<GameDto>(_jsonOptions);

        return gameDto!;
    }

    public async Task<GameDto?> GetGame(ulong discordVoiceChannelId)
    {
        var path = $"/games/{discordVoiceChannelId}";
        try
        {
            return await _httpClient.GetFromJsonAsync<GameDto>(path, _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<GameDto> StartGame(ulong discordVoiceChannelId, List<ulong> players)
    {
        var path = $"/games/{discordVoiceChannelId}/start";

        var responseMessage = await _httpClient.PostAsJsonAsync(path, new
        {
            players
        });

        responseMessage.EnsureSuccessStatusCode();

        var gameDto = await responseMessage.Content.ReadFromJsonAsync<GameDto>(_jsonOptions);

        return gameDto!;
    }

    public async Task<string> ConfirmPlayerRole(ulong? channelId, ulong userId)
    {
        var path = $"/games/{channelId}/players/{userId}/role";

        try
        {
            var roleResponse = await _httpClient.GetFromJsonAsync<GetRoleResponse>(path, _jsonOptions);

            return roleResponse?.Role ?? string.Empty;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
