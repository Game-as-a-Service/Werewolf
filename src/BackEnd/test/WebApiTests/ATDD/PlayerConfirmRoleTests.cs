using System.Collections.Immutable;
using System.Net;
using FastEndpoints;
using NSubstitute;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Entities;
using Wsa.Gaas.Werewolf.Domain.Entities.Rules;
using Wsa.Gaas.Werewolf.Domain.Enums;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Endpoints.Response;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD;

public class PlayerConfirmRoleTests : TestsBase
{
    [Test]
    public async Task PlayerConfirmRole()
    {
        //Init
        var game = GivenGame(gameStatus: GameStatus.Created);
        var manualDelay = new ManualResetEventSlim();

        var playerIds = Enumerable.Range(1, 9)
                                  .Select(o => (long) o)
                                  .ToImmutableList();

        FakeTaskService.Delay(Arg.Any<TimeSpan>())
                       .Returns(_ =>
                                {
                                    manualDelay.Wait();

                                    return Task.CompletedTask;
                                });

        //Before started
        var (responseBeforeStarted, _) = await ExecutePlayerConfirmRole(game);
        responseBeforeStarted!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        //started
        await ExecuteStartGame(game.RoomId, playerIds);
        var (responseAfterStarted, _) = await ExecutePlayerConfirmRole(game);

        responseAfterStarted!.IsSuccessStatusCode.Should().BeTrue();

        await LetBulletFly();

        await FakeTaskService.Received(1)
                             .Delay(TimeSpan.FromSeconds(60));

        //Time up
        HubListenOn(nameof(PlayerRoleConfirmationStoppedEvent));
        manualDelay.Set();
        await LetBulletFly();

        GetGame(game)!.Status.Should().Be(GameStatus.PlayerRoleConfirmationStopped);

        FakeAction.Received(1)
                  .Invoke(Arg.Is<GameVm>(o => o.Id == game.RoomId.ToString()
                                           && o.Status == GameStatus.PlayerRoleConfirmationStopped.ToString()));

        var (responseAfterStopped, _) = await ExecutePlayerConfirmRole(game);
        responseAfterStopped!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task role_count()
    {
        //Init

        var playerIds = Enumerable.Range(1, 9)
                                  .Select(o => (long) o)
                                  .ToImmutableList();

        var game = GivenStartedGame(playerIds);

        var manualDelay = new ManualResetEventSlim();

        FakeTaskService.Delay(Arg.Any<TimeSpan>())
                       .Returns(_ =>
                                {
                                    manualDelay.Wait();

                                    return Task.CompletedTask;
                                });


        await ExecuteStartGame(game.RoomId, playerIds);

        //TODO all role need appear

        var roleCountDict = new Dictionary<string, int>();
        roleCountDict.TryAdd(nameof(Domain.Entities.Rules.Werewolf), 3);
        roleCountDict.TryAdd(nameof(Villager), 3);
        roleCountDict.TryAdd(nameof(Hunter), 1);
        roleCountDict.TryAdd(nameof(Witch), 1);
        roleCountDict.TryAdd(nameof(Seer), 1);

        foreach (var playerId in playerIds)
        {
            var (_, result) = await ExecutePlayerConfirmRole(game, playerId);

            roleCountDict[result!.Role] -= 1;
        }


        foreach (var (role, count) in roleCountDict)
            count.Should().Be(0, $"{role} has wrong count");
    }

    private static async Task LetBulletFly()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    private async Task<(HttpResponseMessage? response, ConfirmPlayerRoleResponse? result)> ExecutePlayerConfirmRole(Game game, long playerId = 1)
    {
        return await HttpClient.GETAsync<ConfirmPlayerRoleRequest, ConfirmPlayerRoleResponse>($"/games/{game.RoomId}/players/{playerId}/Role",
                                                                                              new ConfirmPlayerRoleRequest
                                                                                              {
                                                                                                  RoomId = game.RoomId,
                                                                                                  PlayerId = playerId
                                                                                              });
    }

    private Game GivenGame(GameStatus gameStatus)
    {
        return GameBuilder.WithRandomRoom()
                          .WithGameStatus(gameStatus)
                          .Build();
    }

    private async Task ExecuteStartGame(long roomId, IEnumerable<long> players)
    {
        await HttpClient.POSTAsync<StartGameRequest, StartGameResponse>($"/games/{roomId}/start",
                                                                        new StartGameRequest
                                                                        {
                                                                            RoomId = roomId,
                                                                            Players = players.ToArray()
                                                                        });
    }
}