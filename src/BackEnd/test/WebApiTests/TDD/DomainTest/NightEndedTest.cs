using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.DomainTest;

public class NightEndedTest
{
    [Test]
    [Description("""
        Given:
        * 9 位玩家存活
        * 狼人們沒投票
        * 女巫沒用毒
        
        When:
        * 夜晚結束

        Then:
        * 沒有人死亡，9 位玩家依然存活

        """)]
    public void Given沒狼殺沒用毒_Then平安夜()
    {
        // Arrange
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        // Act: return 平安夜 events
        var events = game.AnnounceNightResult();
        
        // Assert
        foreach(var player in game.Players)
        {
            // 每個人必須活著
            player.IsDead.Should().BeFalse();
            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        events.Should().HaveCount(1);
        events.First().Should().BeOfType<SafetyEveGameEvent>();

    }

    [Test]
    [Description("""
        Given:
        * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        * 狼人們投了 3 號玩家
        * 女巫用了解藥
        
        When:
        * 夜晚結束

        Then:
        * 沒有人死亡，9 位玩家依然存活

        """)]
    public void Given狼殺解藥_Then平安夜()
    {
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        game.Players[3].BuffStatus |= BuffStatus.KilledByWerewolf;
        game.Players[3].BuffStatus |= BuffStatus.SavedByWitch;

        // Act: return 平安夜 events
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            // 每個人必須活著
            player.IsDead.Should().BeFalse();
            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        events.Should().HaveCount(1);
        events.First().Should().BeOfType<SafetyEveGameEvent>();
    }

    [Test]
    [Description("""
        Given:
        * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        * 狼人們投了 5 號玩家
        
        When:
        * 夜晚結束

        Then:
        * 5 號玩家死亡，剩 8 位玩家存活 (1,2,3,4,6,7,8,9)

        """)]
    public void Given只有狼殺_Then一人死亡()
    {
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        var targetPlayer = game.Players[5];
        targetPlayer.BuffStatus |= BuffStatus.KilledByWerewolf;

        // Act: return 玩家死亡
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            if (player.UserId == targetPlayer.UserId)
            {
                player.IsDead.Should().BeTrue();
            }
            else
            {
                player.IsDead.Should().BeFalse();
            }

            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        events.Should().HaveCount(1);
        events.First().Should().BeOfType<PlayerDiedGameEvent>();
    }

    [Test]
    [Description("""
        Given:
        * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        * 狼人們沒投票
        * 女巫毒了 5 號玩家
        
        When:
        * 夜晚結束

        Then:
        * 5 號玩家死亡，剩 8 位玩家存活 (1,2,3,4,6,7,8,9)

        """)]
    public void Given只有用毒_Then一人死亡()
    {
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        var targetPlayer = game.Players[5];
        targetPlayer.BuffStatus |= BuffStatus.KilledByWitch;

        // Act: return 玩家死亡
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            if (player.UserId == targetPlayer.UserId)
            {
                player.IsDead.Should().BeTrue();
            }
            else
            {
                player.IsDead.Should().BeFalse();
            }

            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        events.Should().HaveCount(1);
        events.First().Should().BeOfType<PlayerDiedGameEvent>();
    }

    [Test]
    [Description("""
        Given:
        * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        * 狼人們投了 5 號玩家
        * 女巫毒了 5 號玩家
        
        When:
        * 夜晚結束

        Then:
        * 5 號玩家死亡，剩 8 位玩家存活 (1,2,3,4,6,7,8,9)

        """)]
    public void Given用毒狼殺同一人_Then一人死亡()
    {
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        var targetPlayer = game.Players[5];
        targetPlayer.BuffStatus = BuffStatus.KilledByWitch | BuffStatus.KilledByWerewolf;

        // Act: return 玩家死亡
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            if (player.UserId == targetPlayer.UserId)
            {
                player.IsDead.Should().BeTrue();
            }
            else
            {
                player.IsDead.Should().BeFalse();
            }

            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        events.Should().HaveCount(1);
        events.First().Should().BeOfType<PlayerDiedGameEvent>();
    }
}
