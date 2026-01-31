using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace DiceyParty
{
    public class SessionDataSystem : NetworkBehaviour
    {
        public static Action OnLastPlayerRemoved;
        public static Action<int> OnPlayerInfoRemoved;
        public static SessionDataSystem Instance;
        
        [SerializeField] private GlobalConfigSO _globalConfig;
        
        //Syced To Client
        private readonly SyncDictionary<int, PlayerInfo> _playerData = new();
        private readonly SyncList<int> _clientIds = new(); //List to track the order players joined / who gets host next
        
        //Server-only-access
        private Session _session;
        private HashSet<string> _usedNames = new();
        private Stack<int> _availableColors;
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
        
        public void SetSession(Session session)
        {
            CheckIfServer();
            _session = session;
        }

        public Session GetSession()
        {
            CheckIfServer();
            return _session;
        }

        public PlayerInfo CreatePlayerInfo(int clientId)
        {
            CheckIfServer();

            if (_playerData.TryGetValue(clientId, out var info)) return info;
                
            string initialName = $"Player";
            string playerName = CheckPlayerName(initialName);
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

        private string CheckPlayerName(string initialName)
        {
            string playerName = initialName;
            
            while (_usedNames.Contains(playerName))
            {
                if (Regex.IsMatch(playerName, @"_[0-9]$"))
                {
                    int counter = int.Parse(playerName[^1].ToString());
                    playerName = playerName.Remove(playerName.Length - 1);
                    playerName += (counter + 1).ToString();
                }
                else
                {
                    playerName += "_2";
                }
            }

            _usedNames.Add(playerName);

            return playerName; 
        }
        
        public void TryRemovePlayerInfo(int clientId)
        {
            CheckIfServer();
            if (!_playerData.TryGetValue(clientId, out var player)) return;
            
            _availableColors.Push(player.ColorIndex);
            _playerData.Remove(clientId);
            _usedNames.Remove(player.Name);
            _clientIds.Remove(clientId);
            
            if (_hostId != clientId) return;
            if (_clientIds.Count == 0)
            {
                _hostId = -1;
                OnLastPlayerRemoved?.Invoke();
                return;
            }
            
            _hostId = _clientIds.First();
            var playerInfo = _playerData[_hostId];
            playerInfo.SetIsHost(true);
            OnPlayerInfoRemoved?.Invoke(_hostId);
        }
        
        public void UpdateName(string newName, int clientId)
        {
            CheckIfServer();
            PlayerInfo p = _playerData[clientId];
            _usedNames.Remove(p.Name);
            string adjustedNewName = CheckPlayerName(newName);
            p.SetName(adjustedNewName);
            _playerData.Dirty(clientId);
        }
        
        public IReadOnlyDictionary<int, PlayerInfo> GetPlayerData()
        {
            return _playerData.GetCollection(IsServerInitialized);
        }

        public IReadOnlyList<int> GetClientIds()
        {
            return _clientIds.GetCollection(IsServerInitialized);
        }
        
        private void CheckIfServer()
        {
            if (!IsServerInitialized)
                throw new Exception("This method can not be called on the client");
        }
    }
}

