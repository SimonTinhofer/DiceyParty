using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.GrabABox
{
    public class SpawningManager : NetworkBehaviour
    {
        [SerializeField] private GrabABoxConfigSO _gameConfig;
        [SerializeField] GameObject _playerPrefab;
        [SerializeField] GameObject _boxPrefab;
        
        Dictionary<int, NetworkConnection> _playerConns = new();
        Dictionary<int, NetworkObject> _playerNobs = new();
        List<NetworkObject> _boxNobs = new();

        public override void OnStartServer()
        {
            base.OnStartServer();
            SceneManager.OnClientPresenceChangeEnd += OnClientPresenceChangeEnd;
            _playerConns = ClientManager.Clients;
        }

        private void OnDestroy()
        {
            SceneManager.OnClientPresenceChangeEnd -= OnClientPresenceChangeEnd;
        }

        private void OnClientPresenceChangeEnd(ClientPresenceChangeEventArgs args)
        {
            if(!_playerConns.TryGetValue(args.Connection.ClientId, out NetworkConnection conn))
                _playerConns.Add(args.Connection.ClientId, args.Connection);
        }

        public void SpawnPlayers(List<int> clientIDs)
        {
            _playerNobs.Clear();
            List<Vector2> spawnPoints = PointGenerator.GeneratePoints(_gameConfig.PlayerConstraints[0], _gameConfig.PlayerConstraints[1], _gameConfig.PlayerConstraints[2], _gameConfig.PlayerConstraints[3], clientIDs.Count, _gameConfig.PlayerMinDistance);
            int i = 0;
            foreach(int id in clientIDs)
            {
                Vector3 spawnpoint = new Vector3(spawnPoints[i].x, 0, spawnPoints[i].y);
                i++;
                NetworkConnection conn = _playerConns[id];
                NetworkObject nob = NetworkManager.GetPooledInstantiated(_playerPrefab, spawnpoint, _playerPrefab.transform.rotation, true);
                _playerNobs.Add(conn.ClientId, nob);
                ServerManager.Spawn(nob, conn);
            }
        }

        public void DespawnPlayer(int clientID)
        {
            ServerManager.Despawn(_playerNobs[clientID]);
        }

        public void SpawnSessel(int count)
        {
            _boxNobs.Clear();
            List<Vector2> spawnPoints = PointGenerator.GeneratePoints(_gameConfig.BoxConstraints[0], _gameConfig.BoxConstraints[1], _gameConfig.BoxConstraints[2], _gameConfig.BoxConstraints[3], count, _gameConfig.BoxMinDistance);
            for (int i = 0; i < count; i++)
            {
                Vector3 spawnpoint = new Vector3(spawnPoints[i].x, 10, spawnPoints[i].y);
                NetworkObject nob = NetworkManager.GetPooledInstantiated(_boxPrefab, spawnpoint, _boxPrefab.transform.rotation, true);
                _boxNobs.Add(nob);
                ServerManager.Spawn(nob);
            }
        }

        public void DespawnSessel()
        {
            foreach(NetworkObject nob in _boxNobs)
            {
                ServerManager.Despawn(nob);
            }
        }
    }
}