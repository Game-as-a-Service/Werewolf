using System.Net.Http.Json;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests
{
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

            var request = new GetGameRequest
            {
                DiscordVoiceChannelId = game.DiscordVoiceChannelId,
            };

            // Act
            var response = await _server.Client.GetAsync($"/games/{request.DiscordVoiceChannelId}");

            // Assert
            response.EnsureSuccessStatusCode();

            // 驗證 Restful API 的回應
            var dto = (await response.Content.ReadFromJsonAsync<GetGameResponse>())!;
            dto.Id.Should().Be(request.DiscordVoiceChannelId);
            dto.Status.Should().Be(GameStatus.Started);
            dto.Players.Should().HaveCount(playerCount);

            // 驗證資料庫裡的資料
            var repository = _server.GetRequiredService<IRepository>();
            var actualGame = await repository.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId);
            actualGame!.DiscordVoiceChannelId.Should().Be(request.DiscordVoiceChannelId);
            actualGame.Status.Should().Be(GameStatus.Started);
            actualGame.Players.Should().HaveCount(playerCount);

            // 驗證 SignalR message
            // 但這個測試不需要

        }
    }
}
