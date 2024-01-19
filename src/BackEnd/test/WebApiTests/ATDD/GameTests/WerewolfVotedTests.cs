using FastEndpoints;
using System.Net;
using System.Net.Http.Json;
using Wsa.Gaas.Werewolf.Application;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common;

/* Game
 *   - Players
 *   - 其他東西 
 * 
 * 儲存時一併存進 Repository
 * 
 * 讀取時一併讀取出來 (一次抓好抓滿)
 * Game
 *   - Players
 *   - 其他東西 
 */
namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.GameTests;
internal class WerewolfVotedTests
{
    readonly WebApiTestServer _server = new();

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _server.StartAsync();
    }

    [Test]
    [Description("""
            Given:
            有ABC為狼人
            DEFGHI六個個玩家存活

            When:
            狼人們 ABC 都投票 B 玩家

            Then:
            B 玩家得 3 票
            ACDEFGHI 八個玩家得 0 票
            """)]
    public async Task WerewolfHighestVoteTest()
    {
        // Given, Arrange
        // ATDD 真的需要呼叫 API Endpoints

        // 在 Server 上建立 9 人遊戲 (3 狼人 + 6 玩家)
        var game = _server.CreateGameBuilder()
           .WithRandomDiscordVoiceChannel()
           .WithGameStatus(GameStatus.Started)
           .WithRandomPlayers(9)
           .Build();

        // 找出 3 隻狼人
        var wolves = game.Players
            .Where(x => x.Role is Domain.Objects.Roles.Werewolf)
            .ToList()
            ;

        // 隨機一個倒楣玩家
        Player target = game.Players
            .First(x => x.Role is not Domain.Objects.Roles.Werewolf)
            ;

        // 準備好請求
        var request = new WerewolfVoteRequest
        {
            DiscordChannelId = game.DiscordVoiceChannelId,
            TargetId = target.UserId,
        };

        // When, Act
        // 三隻狼人一起投票
        foreach (var wolf in wolves)
        {
            // Post /games/{gameId}/werewolf/vote
            // {
            //      DiscordChannelId = int
            //      TargetId = int
            //      CallerId = int
            // }

            // 要投票的狼人
            request.CallerId = wolf.UserId;

            // 呼叫 API 的動作
            var httpClient = _server.Client;
            var result = await httpClient
                // FastEndpoint 套件擴充
                .POSTAsync<WerewolfVoteEndpoint, WerewolfVoteRequest, WerewolfVoteResponse>(request);

            // ======================================
            // 沒有套件，要這麼做才能呼叫 API & 拿到回應
            var httpResponseMessage = await httpClient.PostAsync(
                // WerewolfVoteEndpoint
                $"/games/{game.DiscordVoiceChannelId}/werewolf/vote",

                // WerewolfVoteRequest
                JsonContent.Create(new
                {
                    TargetId = target.Id,
                    CallerId = wolf.UserId,
                })
            );

            // WerewolfVoteResponse
            var response = await httpResponseMessage
                .Content.ReadFromJsonAsync<WerewolfVoteResponse>();
            // ======================================


            // 驗證 REST API 得到 200 回應
            result.Response.Should().HaveStatusCode(HttpStatusCode.OK);
            result.Result!.Message.Should().Be("Ok");
        }

        // Then, Assert

        // 要怎麼驗證 
        // 1. 倒楣鬼得到了 3 票
        // 2. 其他人都是 0 票

        // 驗證資料庫是正確的
        var repository = _server.GetRequiredService<IRepository>();
        var actualGame = await repository.FindByDiscordChannelIdAsync(game.DiscordVoiceChannelId);
        actualGame.Should().NotBeNull();

        // 拿最高票的玩家 Id
        actualGame!.VoteManager.GetHighestVotedPlayerId().Should().Be(target.UserId);

        // 最高票應該是 3 票
        actualGame.VoteManager.GetHighestVote().Should().Be(3);

        // 其他 8 人 (5 玩家 + 3 狼人) 得 0 票
        var nonTargets = actualGame.Players
            .Where(x => x != target)
            .ToList()
            ;
        foreach (var nonTarget in nonTargets)
        {
            actualGame.VoteManager.GetPlayerVote(nonTarget.UserId).Should().Be(0);
        }

        // 先不要驗證 SignalR (.NET 版的 Web Socket)
        // 因為狼人投票不應該讓其他玩家知道，所以後端不發送 SignalR
    }

    [Test]
    [Description("""
            Given:
            有1,2,3為狼人
            4,5,6,7,8,9 六個玩家存活
            
            When:
            狼人 1 投 4,
            狼人 2 投 5,
            狼人 3 投 6,
            
            Then:
            玩家 4 得 1 票
            玩家 5 得 1 票
            玩家 6 得 1 票
            系統隨機從 4, 5, 6 淘汰一位
            """)]
    public async Task TieVoteTest()
    {
        // 1. Given / Arrage
        // 在 Server 上建立遊戲
        var game = _server.CreateGameBuilder()
           .WithRandomDiscordVoiceChannel()
           .WithGameStatus(GameStatus.SeerRoundStarted)
           .WithRandomPlayers(9)
           .Build();

        // 找出 3 隻狼人
        var wolves = game.Players
            .Where(x => x.Role is Domain.Objects.Roles.Werewolf)
            .ToList()
            ;

        // 隨機 3 個倒楣玩家
        var targets = game.Players
            .Where(x => x.Role is not Domain.Objects.Roles.Werewolf)
            .Take(3)
            .ToList()
            ;

        // 準備好請求
        var request = new WerewolfVoteRequest
        {
            DiscordChannelId = game.DiscordVoiceChannelId,
        };

        // 2. When / Act
        // await 小軟Act(game, wolves, targets, request);
        // await OliverAct(game, wolves, targets, request);
        await 阿屁達Act(game, wolves, targets, request);

        // 3. Then / Assert 
        // 玩家 4 得 1 票
        // 玩家 5 得 1 票
        // 玩家 6 得 1 票
        // 系統隨機從 4, 5, 6 淘汰一位

        // 驗證資料庫是正確的
        var repository = _server.GetRequiredService<IRepository>();
        var actualGame = await repository.FindByDiscordChannelIdAsync(game.DiscordVoiceChannelId);
        actualGame.Should().NotBeNull();

        // 拿最高票的玩家 Id = 4 or 5 or 6
        targets.Count.Should().Be(3);

        var targetIds = targets
            .Select(x => x.UserId)
            ;

        actualGame!.VoteManager.GetHighestVotedPlayerId().Should().BeOneOf(targetIds);

        // 最高票應該是 1 票
        actualGame!.VoteManager.GetHighestVote().Should().Be(1);

        // 其他 3 人 (5 玩家 + 3 狼人) 得 0 票
        var nonTargets = actualGame.Players
            .Where(x => !targets.Contains(x))
            .ToList();

        foreach (var nonTarget in nonTargets)
        {
            actualGame.VoteManager.GetPlayerVote(nonTarget.UserId).Should().Be(0);
        }
    }

    public async Task 小軟Act(Game game, List<Player> wolves, List<Player> targets, WerewolfVoteRequest request)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            // 要投票的狼人
            request.CallerId = wolves[i].UserId;
            request.TargetId = targets[i].UserId;

            // 呼叫 API 的動作
            var httpClient = _server.Client;
            var result = await httpClient
                .POSTAsync<WerewolfVoteEndpoint, WerewolfVoteRequest, WerewolfVoteResponse>(request);


            // 驗證 REST API 得到 200 回應
            result.Response.Should().HaveStatusCode(HttpStatusCode.OK);
            result.Result!.Message.Should().Be("Ok");
        }
    }

    public async Task OliverAct(Game game, List<Player> wolves, List<Player> targets, WerewolfVoteRequest request)
    {
        for (int i = 0; i < wolves.Count; ++i)
        {
            // Post /games/{gameId}/werewolf/vote
            // {
            //      DiscordChannelId = int
            //      TargetId = int
            //      CallerId = int
            // }

            // 要投票的狼人
            request.CallerId = wolves[i].UserId;


            // 被投票的人
            request.TargetId = targets[i].UserId;

            // 呼叫 API 的動作
            var httpClient = _server.Client;
            var result = await httpClient
                // FastEndpoint 套件擴充
                .POSTAsync<WerewolfVoteEndpoint, WerewolfVoteRequest, WerewolfVoteResponse>(request);


            // 驗證 REST API 得到 200 回應
            result.Response.Should().HaveStatusCode(HttpStatusCode.OK);
            result.Result!.Message.Should().Be("Ok");
        }
    }

    public async Task 阿屁達Act(Game game, List<Player> wolves, List<Player> targets, WerewolfVoteRequest request)
    {
        int cnt = 0;
        foreach (var wolve in wolves)
        {
            request.CallerId = wolve.UserId;
            request.TargetId = targets[cnt].UserId;
            // 呼叫 API 的動作
            var httpClient = _server.Client;
            var result = await httpClient
                // FastEndpoint 套件擴充
                .POSTAsync<WerewolfVoteEndpoint, WerewolfVoteRequest, WerewolfVoteResponse>(request);
            cnt++;

            // 驗證 REST API 得到 200 回應
            result.Response.Should().HaveStatusCode(HttpStatusCode.OK);
            result.Result!.Message.Should().Be("Ok");
        }

    }
}
