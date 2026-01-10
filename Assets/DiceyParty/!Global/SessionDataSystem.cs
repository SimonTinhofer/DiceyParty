using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace DiceyParty
{
    public class SessionDataSystem : NetworkBehaviour
    {
        public static SessionDataSystem Instance;
        [SerializeField] private GlobalConfigSO _globalConfig;

        private readonly SyncDictionary<int, PlayerInfo> _playerData = new();
        private readonly SyncVar<string> _sessionId = new();
        
        private Stack<int> _availableColors;
        private readonly List<int> _clientIds = new(); //List to track the order players joined / who gets host next
        private int _hostId = -1;

        

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("there should only be one instantiated objects of this class in a scene");
                Destroy(this.gameObject);
                return;
            }
        
            Instance = this;
            _availableColors = new Stack<int>(Enumerable.Range(0, _globalConfig.MaxPlayerCount).Reverse());
        }
        
        public void SetSessionId(string sessionId)
        {
            CheckIfServer();
            _sessionId.Value = sessionId;
        }

        public string GetSessionId()
        {
            return _sessionId.Value;
        }

        public PlayerInfo CreatePlayerInfo(int clientId)
        {
            CheckIfServer();

            if (_playerData.TryGetValue(clientId, out var info)) return info;
                
            string playerName = $"Player{clientId}";
            int colorIndex = _availableColors.Pop();
            var player = new PlayerInfo(playerName, colorIndex, clientId);
            _playerData.Add(clientId, player);

            _clientIds.Add(clientId);
            if (_hostId == -1)
            {
                _hostId = clientId;
                player.SetIsHost(true);
            }
            
            return player;
        }
        
        public PlayerInfo RemovePlayerInfo(int clientId)
        {
            CheckIfServer();
            
            if (!_playerData.TryGetValue(clientId, out var player)) return null;
            _availableColors.Push(player.ColorIndex);
            _playerData.Remove(clientId);
            
            _clientIds.Remove(clientId);
            if (_hostId != clientId) return null;
            if (_clientIds.Count > 0)
            {
                _hostId = _clientIds.First();
                var playerInfo = _playerData[_hostId];
                playerInfo.SetIsHost(true);
                return playerInfo;
            }
            
            _hostId = -1;
            
            return null;
        }
        
        public PlayerInfo UpdateName(string newName, int clientId)
        {
            CheckIfServer();
            _playerData[clientId].SetName(newName);
            _playerData.Dirty(clientId);
            return _playerData[clientId];
        }
        
        public Dictionary<int, PlayerInfo> GetPlayerData()
        {
            var pD = _playerData.GetCollection(false);
            return pD;
        }
        
        private void CheckIfServer()
        {
            if (!IsServerInitialized)
                throw new Exception("This method can not be called on the client");
        }
    }
}

