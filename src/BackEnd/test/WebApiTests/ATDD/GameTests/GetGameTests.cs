using Bogus;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests;

using Wsa.Gaas.Werewolf.Application.UseCases;

internal class GetGameTests
{
    readonly WebApiTestServer _server = new();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _server.StartAsync();
    }

    [Test]
    public async Task GetGameTest()
    {
        // Arrange
        var playerCount = new Random().Next(9, 13);
        var game = _server.CreateGameBuilder()
           .WithRandomDiscordVoiceChannel()
           .WithGameStatus(GameStatus.Started)
           .WithRandomPlayers(playerCount)
           .Build();

        // Act
        var response = await _server.Client.GetAsync($"/games/{game.DiscordVoiceChannelId}");

        // Assert
        response.EnsureSuccessStatusCode();

        // 驗證 Restful API 的回應
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter());

        var dto = await response.Content.ReadFromJsonAsync<GetGameResponse>(options);
        dto!.Id.Should().Be(game.DiscordVoiceChannelId);
        dto.Status.Should().Be(GameStatus.Started);
        dto.Players.Should().HaveCount(playerCount);

        // 驗證資料庫裡的資料
        var repository = _server.GetRequiredService<IRepository>();
        var actualGame = await repository.FindByDiscordChannelIdAsync(game.DiscordVoiceChannelId);
        actualGame!.DiscordVoiceChannelId.Should().Be(game.DiscordVoiceChannelId);
        actualGame.Status.Should().Be(GameStatus.Started);
        actualGame.Players.Should().HaveCount(playerCount);

        // 驗證 SignalR message
        // 但這個測試不需要

    }

    [Test]
    public async Task GetNonExistGame_ShouldReturn404()
    {
        var faker = new Faker();

        // Arrange
        var voiceChannelId = faker.Random.ULong();

        // Act
        var response = await _server.Client.GetAsync($"/games/{voiceChannelId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
