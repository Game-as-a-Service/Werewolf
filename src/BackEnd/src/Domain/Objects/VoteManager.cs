namespace Wsa.Gaas.Werewolf.Domain.Objects;
public class VoteManager
{
    // Dic<投票的玩家, 被投的玩家>
    private readonly Dictionary<ulong, ulong> votes = new();
    // Dic<被投的玩家, 票數>
    public readonly Dictionary<ulong, int> voteResult = new();

    public void Clear()
    {
        votes.Clear();
        voteResult.Clear();
    }

    // 計票
    public void Vote(ulong voterId, ulong voteeId)
    {
        // 投過票了嗎? 先把投票取消，在記錄新投票
        if (votes.ContainsKey(voterId))
        {
            voteResult[voteeId]--;
        }

        // 紀錄投票
        votes[voterId] = voteeId;

        // 統計得票數
        if (voteResult.ContainsKey(voteeId) == false)
        {
            voteResult[voteeId] = 0;
        }
        voteResult[voteeId]++;
    }

    // 回傳最高票數的玩家 Id
    public ulong? GetHighestVotedPlayerId()
    {
        return GetPlayerIdToBeKilled(voteResult);
    }

    // 回傳最高票數
    public int GetHighestVote()
    {
        return voteResult.Any() ? voteResult.Values.Max() : 0;
    }

    // 回傳玩家的得票數
    public int GetPlayerVote(ulong playerId)
    {
        return voteResult.ContainsKey(playerId) ? voteResult[playerId] : 0;
    }

    // nightVotes : UserId => 得票數
    internal ulong? GetPlayerIdToBeKilled(Dictionary<ulong, int> nightVotes)
    {
        // 平安夜
        if (nightVotes.Values.Sum() == 0)
        {
            return null;
        }

        // 最高得票數
        var maxVotes = nightVotes.Values.Max();

        // 拿到最高得票數的玩家們
        var maxVotePlayers = nightVotes
            .Where(x => x.Value == maxVotes)
            .Select(kv => kv.Key);

        // 有平票則隨機選玩家出局
        // 沒有平票則選最高票的玩家出局
        return maxVotePlayers
            .OrderBy(_ => Guid.NewGuid())
            .First();
    }
}