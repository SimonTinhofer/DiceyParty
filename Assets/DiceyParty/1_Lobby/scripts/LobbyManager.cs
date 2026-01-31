using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.Lobby
{
    public class LobbyManager : NetworkBehaviour
    {
        private static LobbyManager _instance;
        [SerializeField] private LobbyUIHandler _lobbyUIHandler;
        [SerializeField] private GameObject _playerCardPrefab; 
        [SerializeField] private PlayerCardManager _playerCardManager;
    
        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogWarning("there should only be one instantiated objects of this class in a scene");
                Destroy(this.gameObject);
            }
            else
                _instance = this;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Session session = SessionDataSystem.Instance.GetSession();
            Debug.Log(session.Name);
            ObserverSetSessionName(session.Name);

            SceneManager.OnClientPresenceChangeEnd += OnClientPresenceChangeEnd;
            SessionDataSystem.OnPlayerInfoRemoved += OnPlayerInfoRemoved;
        }
        
        private void OnDestroy()
        {
            SceneManager.OnClientPresenceChangeEnd -= OnClientPresenceChangeEnd;
            SessionDataSystem.OnPlayerInfoRemoved -= OnPlayerInfoRemoved;
        }
        
        private void OnClientPresenceChangeEnd(ClientPresenceChangeEventArgs args)
        {
            NetworkConnection conn = args.Connection;
            int clientId = conn.ClientId;
            PlayerInfo playerInfo = SessionDataSystem.Instance.CreatePlayerInfo(clientId);
            _playerCardManager.SetupPlayerCardsServer();

            if (playerInfo.IsHost)
            {
                TargetEnablePlayButton(conn);
            }
        }
        
        private void OnPlayerInfoRemoved(int newHostId)
        {
            _playerCardManager.SetupPlayerCardsServer();
            NetworkConnection hostConn = ServerManager.Clients[newHostId];
            TargetEnablePlayButton(hostConn);
        }

        [TargetRpc]
        private void TargetEnablePlayButton(NetworkConnection conn)
        {
            _lobbyUIHandler.EnablePlayButton();
        }

        [ObserversRpc (BufferLast = true)]
        private void ObserverSetSessionName(string sessionName)
        {
            _lobbyUIHandler.SetSessionName(sessionName);
        }

        public static void UpdateName(string newName) => _instance.HandleUpdateName(newName);

        private void HandleUpdateName(string newName)
        {
            ServerUpdateName(newName, ClientManager.Connection.ClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ServerUpdateName(string newName, int clientId)
        {
            SessionDataSystem.Instance.UpdateName(newName, clientId);
            _playerCardManager.SetupPlayerCardsServer();
        }

        public static void PlayMiniGame(int sceneIndex) => _instance.ServerPlayMiniGame(sceneIndex);

        [ServerRpc (RequireOwnership = false)]
        private void ServerPlayMiniGame(int sceneIndex)
        {
            SceneManager.OnClientPresenceChangeEnd -= OnClientPresenceChangeEnd;
            SessionStageSystem.SetNextMiniGame(sceneIndex);
            SessionStageSystem.ChangeState(SessionStage.MiniGame);
        }
    }
}