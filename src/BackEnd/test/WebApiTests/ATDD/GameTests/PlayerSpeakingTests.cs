using System.Threading.Tasks.Dataflow;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests;

public class PlayerSpeakingTests
{
    WebApiTestServer _server = new();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _server.StartAsync();
    }

    [Test]
    public async Task PeaceNightTest()
    {
        _server.ListenOn<PlayerSpeakingEvent>();

        var givenGame = _server.CreateGameBuilder()
                               .WithRandomDiscordVoiceChannel()
                               .WithRandomPlayers(9)
                               .WithGameStatus(GameStatus.LastNightResultAnnounced)
                               .Build();

        //When
        await _server.GetRequiredService<Policy<LastNightResultAnnouncedEvent>>()
                     .ExecuteAsync(new LastNightResultAnnouncedEvent(givenGame));

        //Then

        var game = await _server.GetRequiredService<IRepository>()
                                .FindByIdAsync(givenGame.Id);

        var gameEvent = await _server.EventBuffer.ReceiveAsync();

        game!.Status.Should().Be(GameStatus.PlayerSpeaking);
        game.Status.ToString().Should().Be(gameEvent.Status);

        game.CurrentSpeakingPlayer.Should().NotBeNull();
        game.CurrentSpeakingPlayer!.UserId.Should().Be(gameEvent.CurrentSpeakingPlayer);

        game.DiscordVoiceChannelId.ToString().Should().Be(gameEvent.Id);
    }
}