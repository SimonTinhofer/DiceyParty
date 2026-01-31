using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.CoinDilemma
{
    public class PlayerScoreManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Transform _containerTransform;
        private Dictionary<int, PlayerScoreHandler> _handlers = new();

        public void Setup()
        {
            var playerScoreData = CreatePlayerScoreData();
            SetupObservers(playerScoreData);
        }

        private Dictionary<int, PlayerScoreInfo> CreatePlayerScoreData()
        {
            Dictionary<int, PlayerScoreInfo> playerScoreData = new();
            var playerData = SessionDataSystem.Instance.GetPlayerData();
            foreach (var pair in playerData)
            {
                int clientId = pair.Key;
                int colorIndex = pair.Value.ColorIndex;
                PlayerScoreInfo info = new(clientId, colorIndex, 0);
                playerScoreData.Add(clientId, info);
            }

            return playerScoreData;
        }

        [ObserversRpc (RunLocally = true, BufferLast = true)]
        private void SetupObservers(Dictionary<int, PlayerScoreInfo> playerScoreData)
        {
            int localClientId = LocalConnection.ClientId;
            foreach (var pair in playerScoreData)
            {
                int clientId = pair.Key;
                PlayerScoreInfo p = pair.Value;
                var go = Instantiate(_prefab, _containerTransform);
                var handler = go.gameObject.GetComponent<PlayerScoreHandler>();
                handler.Setup(p, localClientId);
                _handlers.Add(clientId, handler);
            }
        }

        public void UpdateCoinAmounts(Dictionary<int, int> coinAmounts)
        {
            UpdateCoinAmountsObservers(coinAmounts);
        }

        [ObserversRpc (RunLocally = true, BufferLast = true)]
        private void UpdateCoinAmountsObservers(Dictionary<int, int> coinAmounts)
        {
            foreach (var pair in coinAmounts)
            {
                int clientId = pair.Key;
                int coinAmount = pair.Value;
                _handlers[clientId].UpdateCoinAmount(coinAmount);
            }
        }
    }
}