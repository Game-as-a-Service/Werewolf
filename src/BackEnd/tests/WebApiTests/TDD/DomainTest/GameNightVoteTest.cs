namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.DomainTest;
public class GameNightVoteTest
{
    //[Test]
    //public void PeaceNightTest()
    //{
    //    ulong discordChannelId = 1;

    //    // Given 狼人都沒有投票
    //    var game = new Game(discordChannelId);
    //    game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
    //    game.nightVotes = game.Players.ToDictionary(p => p, p => 0);

    //    // When 投票結束，計算夜晚得票數
    //    game.CalculateNightVotes();

    //    // Then 沒有人出局
    //    game.nightKilledPlayerId.Should().BeNull();
    //}

    //[Test]
    //public void HighestVoteTest()
    //{
    //    var random = new Random();
    //    ulong discordChannelId = 1;

    //    // Given 
    //    var game = new Game(discordChannelId);
    //    // input 給玩家 PlayerId, 加入後會隨機給玩家號碼 PlayerNumber
    //    game.AddPlayers(new ulong[] { 11, 22, 33, 44, 55, 66, 77, 88, 99 });

    //    // 3 號玩家1 票
    //    // 6 號玩家2 票
    //    game.nightVotes = game.Players.ToDictionary(
    //        p => p, 
    //        p => p.PlayerNumber == 3 ? 1 : p.PlayerNumber == 6 ? 2 : 0
    //    );

    //    // When 投票結束，計算夜晚得票數
    //    game.CalculateNightVotes();

    //    // Then 6 號玩家出局
    //    var expectedPlayerId = game.Players.First(p => p.PlayerNumber == 6).UserId;
    //    game.nightKilledPlayerId.Should().Be(expectedPlayerId);
    //}
}
