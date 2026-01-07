using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class PaintTheBallManager : NetworkBehaviour
    {
        private readonly Dictionary<int, int> _placements = new();
        private int _playerCount;

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameConfigSO _gameConfig;
        


        public override void OnStartServer()
        {
            base.OnStartServer();
            SceneManager.OnClientPresenceChangeEnd += SpawnPlayer;
            _playerCount = SessionDataSystem.GetPlayerCount();
            
            MiniGameManager.OnStartGamePhase += OnStartGamePhase;
        }

        private void SpawnPlayer(ClientPresenceChangeEventArgs args)
        {
            NetworkConnection conn = args.Connection;
            NetworkObject nob = NetworkManager.GetPooledInstantiated(_playerPrefab, new Vector3(0, 1, -_gameConfig.Radius), Quaternion.identity, true);
            NetworkManager.ServerManager.Spawn(nob, conn);
        }


        private void OnDestroy()
        {
            SceneManager.OnClientPresenceChangeEnd += SpawnPlayer;
            MiniGameManager.OnStartGamePhase -= OnStartGamePhase;
        }

        private void OnStartGamePhase()
        {
            
        }
        
        [ServerRpc (RequireOwnership = false)] 
        private void FinishedGamePhase(int clientId)
        {
            _placements.Add(clientId, _placements.Count);
            if (_placements.Count == _playerCount)
            {
                MiniGameManager.FinishedGamePhase(_placements);
            }
        }
    }
}