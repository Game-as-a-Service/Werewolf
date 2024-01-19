using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.DomainTest;
public class WerewolfVoteTests
{
    [Test]
    [Description("""
            Given:
            有1,2,3為狼人
            4,5,6,7,8,9 六個玩家存活

            When:
            狼人們 1,2,3 都投票 4 玩家

            Then:
            4 玩家得 3 票
            1,2,3,5,6,7,8,9 八個玩家得 0 票
            """)]
    public void WerewolfVote_HighestVoteOut()
    {
        // 1. Arrange / Given / 前置作業
        var game = new Game();
        // 開始遊戲 9 個玩家，並 assign 角色 (3 狼人 / 3 神職 / 3 平民)
        game.StartGame(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        // TODO: 應該要開方法可以直接指定腳色


        // 2. Act / When / 測試動作
        var werewolves = game.Players
            .Where(x => x.Role is Domain.Objects.Roles.Werewolf)
            .ToList();

        var target = game.Players
            .First(x => x.Role is not Domain.Objects.Roles.Werewolf);

        foreach (var werewolf in werewolves)
        {
            game.WerewolfVote(werewolf.UserId, target.UserId);
        }

        // 3. Assert / Then / 查驗結果
        game.VoteManager.GetHighestVotedPlayerId().Should().Be(target.UserId);
        game.VoteManager.GetHighestVote().Should().Be(werewolves.Count);

        foreach (var player in game.Players)
        {
            var expectedVote = player.UserId == target.UserId ? 3 : 0;

            game.VoteManager.GetPlayerVote(player.UserId).Should().Be(expectedVote);
        }
    }

    [Test]
    [Description("""
            Given:
            有1,2,3為狼人
            4,5,6,7,8,9 六個玩家存活
            
            When:
            狼人們 1,2,3 都沒投票
            
            Then:
            1,2,3,4, 5,6,7,8,9 八個玩家得 0 票
            """)]
    public void WerewolfVote_NoOneOut()
    {
        // 1. Arrange / Given / 前置作業
        var game = new Game();
        // 開始遊戲 9 個玩家，並 assign 角色 (3 狼人 / 3 神職 / 3 平民)
        game.StartGame(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        // TODO: 應該要開方法可以直接指定腳色


        // 2. Act / When / 測試動作
        // Do Nothing

        // 3. Assert / Then / 查驗結果
        game.VoteManager.GetHighestVote().Should().Be(0);

        foreach (var player in game.Players)
        {
            var expectedVote = 0;
            game.VoteManager.GetPlayerVote(player.UserId).Should().Be(expectedVote);
        }
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
    public void WerewolfVote_TieResult_RandomPlayerOut()
    {
        // 1. Arrange / Given / 前置作業
        var game = new Game();
        // 開始遊戲 9 個玩家，並 assign 角色 (3 狼人 / 3 神職 / 3 平民)
        game.StartGame(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        // TODO: 應該要開方法可以直接指定腳色


        // 2. Act / When / 測試動作
        var werewolves = game.Players
            .Where(x => x.Role is Domain.Objects.Roles.Werewolf)
            .ToList();

        // C# LINQ
        var targets = game.Players
            // 6 位
            .Where(x => x.Role is not Domain.Objects.Roles.Werewolf)
            // 拿前 3 位
            .Take(werewolves.Count)
            .ToList()
            ;

        for (int i = 0; i < targets.Count; i++)
        {
            game.WerewolfVote(werewolves[i].UserId, targets[i].UserId);
        }

        // 3. Assert / Then / 查驗結果
        var targetIds = targets.Select(x => x.UserId);
        game.VoteManager.GetHighestVotedPlayerId().Should().BeOneOf(targetIds);
        game.VoteManager.GetHighestVote().Should().Be(1);

        foreach (var player in game.Players)
        {
            var expectedVote = targetIds.Contains(player.UserId) ? 1 : 0;
            game.VoteManager.GetPlayerVote(player.UserId).Should().Be(expectedVote);
        }
    }
}
