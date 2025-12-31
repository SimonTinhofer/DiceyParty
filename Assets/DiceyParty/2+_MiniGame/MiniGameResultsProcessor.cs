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
                int clientID = entry.Key;
                int placement = entry.Value;
                PlayerInfo p = playerData[clientID];
                ResultCardInfo info = new(p.PlayerName, placement);
                resultCardData.Add(clientID, info);
            }
            return resultCardData;
        }
    }
}