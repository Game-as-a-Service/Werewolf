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
    public async Task player_confirm_role()
    {
        //Init
        var game = GivenCreatedGame();
        var manualStopAction = GivenConfirmRoleDurationByManual();

        //Before started
        var (responseBeforeStarted, _) = await ExecutePlayerConfirmRole(game);
        responseBeforeStarted!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        //started
        await ExecuteStartGame(game.RoomId, GetPlayerIds(9));
        var (responseAfterStarted, _) = await ExecutePlayerConfirmRole(game);
        responseAfterStarted!.IsSuccessStatusCode.Should().BeTrue();
        await LetBulletFly();

        await FakeTaskService.Received(1)
                             .Delay(TimeSpan.FromSeconds(60));

        //Time up
        HubListenOn(nameof(PlayerRoleConfirmationStoppedEvent));
        manualStopAction.Invoke();
        await LetBulletFly();
        GetGame(game)!.Status.Should().Be(GameStatus.PlayerRoleConfirmationStopped);

        FakeAction.Received(1)
                  .Invoke(Arg.Is<GameVm>(o => o.Id == game.RoomId.ToString()
                                           && o.Status == GameStatus.PlayerRoleConfirmationStopped.ToString()));

        var (responseAfterStopped, _) = await ExecutePlayerConfirmRole(game);
        responseAfterStopped!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [TestCase(9)]
    [TestCase(10)]
    [TestCase(11)]
    [TestCase(12)]
    public async Task role_count(int playerCount)
    {
        //Init
        _ = GivenConfirmRoleDurationByManual();
        var playerIds = GetPlayerIds(playerCount);
        var game = GivenStartedGame(playerIds);
        var roleCountDict = GetRoleCountDict(playerCount);

        //Assert
        foreach (var playerId in playerIds)
        {
            var (_, result) = await ExecutePlayerConfirmRole(game, playerId);
            roleCountDict[result!.Role] -= 1;
        }

        foreach (var (role, count) in roleCountDict)
            count.Should().Be(0, $"{role} has wrong count");
    }

    private static ImmutableList<long> GetPlayerIds(int playerCount)
    {
        return Enumerable.Range(1, playerCount)
                         .Select(o => (long) o)
                         .ToImmutableList();
    }

    private static Dictionary<string, int> GetRoleCountDict(int playerCount)
    {
        var roleCountDict = new Dictionary<string, int>
                            {
                                { nameof(Domain.Entities.Rules.Werewolf), 3 },
                                { nameof(Villager), 3 },
                                { nameof(Hunter), 1 },
                                { nameof(Witch), 1 },
                                { nameof(Seer), 1 }
                            };

        if (playerCount >= 10)
            roleCountDict[nameof(Villager)] += 1;

        if (playerCount >= 11)
            roleCountDict.Add(nameof(AlphaWerewolf), 1);

        if (playerCount >= 12)
            roleCountDict.Add(nameof(Guardian), 1);

        return roleCountDict;
    }

    private Action GivenConfirmRoleDurationByManual()
    {
        var manualDelay = new ManualResetEventSlim();

        FakeTaskService.Delay(Arg.Any<TimeSpan>())
                       .Returns(_ =>
                                {
                                    manualDelay.Wait();

                                    return Task.CompletedTask;
                                });

        return manualDelay.Set;
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