using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.RollOff
{
    public class PlayerScoreManager : NetworkBehaviour
    {
        public static PlayerScoreManager Instance;

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }

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

        [ServerRpc(RequireOwnership = false)]
        public void UpdateLongestRunServer(int clientId, int longestRun)
        {
            UpdateLongestRunObservers(clientId, longestRun);
        }

        [ObserversRpc (RunLocally = true, BufferLast = true)]
        private void UpdateLongestRunObservers(int clientId, int longestRun)
        {
            _handlers[clientId].UpdateLongestRun(longestRun);
        }
    }
}