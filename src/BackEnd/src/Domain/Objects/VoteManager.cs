namespace Wsa.Gaas.Werewolf.Domain.Objects
{
    public class VoteManager
    {
        // Dic<投票的玩家, 被投的玩家>
        private readonly Dictionary<ulong, ulong> votes = new();
        // Dic<被投的玩家, 票數>
        public readonly Dictionary<ulong, int> voteResult = new();

        public void Clear()
        {
        }

        // 計票
        public void Vote(ulong voterId, ulong voteeId)
        {
            // 投過票了嗎?
            if (votes.ContainsKey(voterId))
            {
                throw new Exception("Voted");
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
            else
            {
                // 最高票的玩家出局                
                var maxVotes = nightVotes.Values.Max();
                var maxVotePlayers = nightVotes
                    .Where(x => x.Value == maxVotes)
                    .Select(kv => kv.Key);
                var isTie = maxVotePlayers.Count() > 1;


                // 有平票 random
                if (isTie)
                {
                    // 隨機從最高票的玩家中選一個出局 方法1
                    return maxVotePlayers
                        .OrderBy(_ => Guid.NewGuid())
                        .First();
                }
                else // 沒有平票
                {
                    return nightVotes
                        .FirstOrDefault(x => x.Value == maxVotes).Key;
                }
            }
        }

    }
}