using System.Collections.Generic;
using FishNet.Object;

namespace DiceyParty.MiniGame
{
    public class MiniGameResultsProcessor : NetworkBehaviour
    {
        /// <param name="placements">Dictionary: Key = clientId, Value = placement</param>
        /// <returns></returns>
        public Dictionary<int, ResultCardInfo> ProcessResults(Dictionary<int, int> placements)
        {
            Dictionary<int, ResultCardInfo> resultCardData = new();
            Dictionary<int, PlayerInfo> playerData = SessionDataSystem.GetPlayerData();
            foreach (var entry in placements)
            {
                int clientId = entry.Key;
                int placement = entry.Value;
                PlayerInfo p = playerData[clientId];
                ResultCardInfo info = new(p.PlayerName, placement);
                resultCardData.Add(clientId, info);
            }
            return resultCardData;
        }
    }
}