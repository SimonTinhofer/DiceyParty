using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.RollOff
{
    public class RollOffManager : NetworkBehaviour
    {
        public static RollOffManager Instance;
        public static Action<bool> OnTogglePlayerControls;
        
        public Transform Spawnpoint;

        [SerializeField] private RollOffConfigSO _gameConfig;
        [SerializeField] private GameObject _playerPrefab;

        private Dictionary<int, int> _results = new();

        private void Awake()
        {
            if(Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SceneManager.OnClientPresenceChangeEnd += SpawnPlayer;
            MiniGameManager.OnStartGamePhase += StartGame;
        }

        private void OnDestroy()
        {
            OnTogglePlayerControls = null;
            SceneManager.OnClientPresenceChangeEnd -= SpawnPlayer;
            MiniGameManager.OnStartGamePhase -= StartGame;
        }
        
        private void SpawnPlayer(ClientPresenceChangeEventArgs args)
        {
            NetworkConnection conn = args.Connection;
            NetworkObject nob = NetworkManager.GetPooledInstantiated(_playerPrefab, Spawnpoint.transform.position, Quaternion.identity, true);
            NetworkManager.ServerManager.Spawn(nob, conn);
        }
        
        private void StartGame()
        {
            StartGameObserver();
        }

        [ObserversRpc]
        private void StartGameObserver()
        {
            UIManager.Instance.StartTimer(_gameConfig.GameDuration);
            OnTogglePlayerControls?.Invoke(true);
            TimeGamePhase();
        }

        private async void TimeGamePhase()
        {
            await Awaitable.WaitForSecondsAsync(_gameConfig.GameDuration);
            EndGamePhase();
        }

        private void EndGamePhase()
        {
            OnTogglePlayerControls?.Invoke(false);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void ClientFinishedGamePhase(int clientId, int longestRun)
        {
            _results.Add(clientId, longestRun);
            if (_results.Count == SessionDataSystem.Instance.GetClientIds().Count)
            {
                ProcessResults();
            }
        }

        private void ProcessResults()
        {
            var  orderedResults = _results.OrderByDescending(pair => pair.Value);
            Dictionary<int, int> placements =  orderedResults.Select((pair, index) => new { pair.Key, Rank = orderedResults.Count(p => p.Value > pair.Value)}).ToDictionary(pair => pair.Key, pair => pair.Rank);
            MiniGameManager.FinishedGamePhase(placements);
        }
    }
}