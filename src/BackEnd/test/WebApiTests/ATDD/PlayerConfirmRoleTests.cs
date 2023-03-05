using System.Collections.Immutable;
using System.Net;
using FastEndpoints;
using NSubstitute;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Entities;
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

        //TODO all role need appear

        var confirmRoleTasks = playerIds.Select(playerId =>
                                                    Task.Run(async () =>
                                                             {
                                                                 var (resp, result) = await ExecutePlayerConfirmRole(game, playerId);

                                                                 resp!.IsSuccessStatusCode
                                                                      .Should().BeTrue();

                                                                 result.Should()
                                                                       .BeEquivalentTo(new
                                                                                       {
                                                                                           GameId = game.RoomId.ToString(),
                                                                                           PlayerId = playerId.ToString(),
                                                                                       });
                                                             }));

        await Task.WhenAll(confirmRoleTasks);


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