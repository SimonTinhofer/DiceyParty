using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class PaintTheBallManager : NetworkBehaviour
    {
        public static Action<bool> TogglePlayerControls;
        
        private readonly HashSet<int> _readyPlayers = new();
        private readonly SyncVar<int> _playerCount = new SyncVar<int>();

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private PaintTheBallConfigSO _paintTheBallConfig;
        private int _clientId;


        public override void OnStartServer()
        {
            base.OnStartServer();
            SceneManager.OnClientPresenceChangeEnd += SpawnPlayer;
            _playerCount.Value = SessionDataSystem.Instance.GetPlayerData().Count;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            _clientId = ClientManager.Connection.ClientId;
            MiniGameManager.OnStartGamePhase += OnStartGamePhase;
        }

        private void SpawnPlayer(ClientPresenceChangeEventArgs args)
        {
            NetworkConnection conn = args.Connection;
            NetworkObject nob = NetworkManager.GetPooledInstantiated(_playerPrefab, new Vector3(_paintTheBallConfig.Radius, 0, 0), Quaternion.identity, true);
            NetworkManager.ServerManager.Spawn(nob, conn);
        }


        private void OnDestroy()
        {
            SceneManager.OnClientPresenceChangeEnd -= SpawnPlayer;
            MiniGameManager.OnStartGamePhase -= OnStartGamePhase;
            TogglePlayerControls = null;
        }

        private void OnStartGamePhase()
        {
            UIManager.StartTimer(_paintTheBallConfig.GameDuration);
            WaitGameDuration();
            TogglePlayerControls.Invoke(true);
        }

        private async void WaitGameDuration()
        {
            await Awaitable.WaitForSecondsAsync(_paintTheBallConfig.GameDuration);
            TogglePlayerControls.Invoke(false);
            FinishedGamePhase(_clientId);
        }

        [ServerRpc (RequireOwnership = false)] 
        private void FinishedGamePhase(int clientId)
        {
            _readyPlayers.Add(clientId);
            if (_readyPlayers.Count == _playerCount.Value)
            {
                var placements = CalculatePlacements();
                MiniGameManager.FinishedGamePhase(placements);
            }
        }

        private Dictionary<int, int> CalculatePlacements()
        {
            var playerTriCount = TriangleManager.GetPlayerTriangleCount();
            IOrderedEnumerable<KeyValuePair<int, int>> orderedPlayerTriCount = playerTriCount.OrderByDescending(entry => entry.Value);
            Dictionary<int, int> placements = orderedPlayerTriCount.Select((pair, index) => new { pair.Key, Rank = index }).ToDictionary(pair => pair.Key, pair => pair.Rank);
            return placements;
        }
    }
}