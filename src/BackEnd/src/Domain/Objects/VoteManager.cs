namespace Wsa.Gaas.Werewolf.Domain.Objects
{
    public class VoteManager
    {
        private readonly Dictionary<ulong, ulong> votes = new ();

        public void Vote(ulong voterId, ulong voteeId)
        {
            // 投過票了嗎?
            if(votes.ContainsKey(voterId))
            {
                throw new Exception("Voted");
            }

            // store vote
            votes[voterId] = voteeId;
        }

        public ulong? GetHighestVotedPlayerId()
        {
            var nightVotes = new Dictionary<ulong, int>();

            foreach (var kv in votes)
            {
                if(nightVotes.ContainsKey(kv.Key) == false)
                {
                    nightVotes[kv.Key] = 0;
                }
                nightVotes[kv.Key]++;
            }

            return CalculateNightVotes(nightVotes);

        }

        internal ulong? CalculateNightVotes(Dictionary<ulong, int> nightVotes)
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