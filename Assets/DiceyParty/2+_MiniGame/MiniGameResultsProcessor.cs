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
            IReadOnlyDictionary<int, PlayerInfo> playerData = SessionDataSystem.Instance.GetPlayerData();
            foreach (var entry in placements)
            {
                int clientId = entry.Key;
                int placement = entry.Value;
                PlayerInfo p = playerData[clientId];
                ResultCardInfo info = new(p.Name, placement, p.ColorIndex);
                resultCardData.Add(clientId, info);
            }
            return resultCardData;
        }
    }
}