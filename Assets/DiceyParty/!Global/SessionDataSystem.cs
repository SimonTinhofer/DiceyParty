using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty
{
    public class SessionDataSystem : NetworkBehaviour
    {
        private static SessionDataSystem _instance;
        [SerializeField] private GlobalConfigSO _globalConfig;
        
        private Dictionary<int, PlayerInfo> _playerData = new ();
        private string _sessionId;
        private Stack<int> _availableColors;
        private List<int> _clientIds = new(); //List to track the order players joined / who gets host next
        private int _hostId = -1;

        

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogWarning("there should only be one instantiated objects of this class in a scene");
                Destroy(this.gameObject);
                return;
            }
        
            _instance = this;
            _availableColors = new Stack<int>(Enumerable.Range(0, _globalConfig.MaxPlayerCount).Reverse());
        }
    
        public static void SetSessionId(string sessionId)
        {
            _instance.CheckIfServer();
            _instance._sessionId = sessionId;
        }

        public static string GetSessionId()
        {
            return _instance._sessionId;
        }

        public static PlayerInfo AddPlayerInfo(int clientId) => _instance.HandleAddPlayerInfo(clientId);
        
        private PlayerInfo HandleAddPlayerInfo(int clientId)
        {
            CheckIfServer();
            
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
        
        public static PlayerInfo RemovePlayerInfo(int clientId) => _instance.HandleRemovePlayerInfo(clientId);
        
        private PlayerInfo HandleRemovePlayerInfo(int clientId)
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
        
        public static PlayerInfo UpdateName(string newName, int clientId) => _instance.HandleUpdateName(newName, clientId);

        private PlayerInfo HandleUpdateName(string newName, int clientId)
        {
            CheckIfServer();
            _playerData[clientId].SetName(newName);
            return _playerData[clientId];
        }

        public static int GetPlayerCount() => _instance.HandleGetPlayerCount();

        private int HandleGetPlayerCount()
        {
            CheckIfServer();
            return _playerData.Count;
        }

        public static Dictionary<int, PlayerInfo> GetPlayerData() => _instance.HandleGetPlayerData();

        private Dictionary<int, PlayerInfo> HandleGetPlayerData()
        {
            CheckIfServer();
            return _playerData;
        }

        private void CheckIfServer()
        {
            if (!IsServerInitialized)
                throw new Exception("This method can not be called on the client");
        }
    }
    
    public class PlayerInfo
    {
        public string PlayerName { get; private set; }
        public int ColorIndex { get; private set; }
        public int ClientId { get; private set; }
        public bool IsHost { get; private set; }

        public PlayerInfo(string playerName, int colorIndex, int clientId)
        {
            PlayerName = playerName;
            ColorIndex = colorIndex;
            ClientId = clientId;
        }

        public void SetIsHost(bool toggle)
        {
            IsHost = toggle;
        }

        public void SetName(string newName)
        {
            PlayerName = newName;
        }
    }
}

