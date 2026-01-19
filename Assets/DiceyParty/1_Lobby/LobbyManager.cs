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

        private Dictionary<int, PlayerCardHandler> _playerCardHandlers = new ();
    
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
        
        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Session session = SessionDataSystem.Instance.GetSession();
            Debug.Log(session.Name);
            ObserverSetSessionName(session.Name);

            SceneManager.OnClientPresenceChangeEnd += OnClientPresenceChangeEnd;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnClientPresenceChangeEnd(ClientPresenceChangeEventArgs args)
        {
            NetworkConnection conn = args.Connection;
            int clientId = conn.ClientId;
            PlayerInfo playerInfo = SessionDataSystem.Instance.CreatePlayerInfo(clientId);
            
            NetworkObject nob = NetworkManager.GetPooledInstantiated(_playerCardPrefab, true);
            NetworkManager.ServerManager.Spawn(nob, conn);
            var handler = nob.gameObject.GetComponent<PlayerCardHandler>();
            handler.SetupServer(playerInfo);
            _playerCardHandlers.Add(args.Connection.ClientId, handler);

            if (playerInfo.IsHost)
            {
                TargetEnablePlayButton(conn);
            }
        }

        [TargetRpc]
        private void TargetEnablePlayButton(NetworkConnection conn)
        {
            _lobbyUIHandler.EnablePlayButton();
        }

        private void OnDestroy()
        {
            SceneManager.OnClientPresenceChangeEnd -= OnClientPresenceChangeEnd;
        }

        [ObserversRpc (BufferLast = true)]
        private void ObserverSetSessionName(string sessionName)
        {
            _lobbyUIHandler.SetSessionName(sessionName);
        }

        public static void LeaveSession() => _instance.HandleLeaveSession();
        
        private void HandleLeaveSession()
        {
            ServerLeaveSession(ClientManager.Connection.ClientId, ClientManager.Connection);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ServerLeaveSession(int clientId, NetworkConnection conn)
        {
            Debug.Log("Removing Player Info");
            var updatePlayerInfo = SessionDataSystem.Instance.TryRemovePlayerInfo(clientId);
            _playerCardHandlers.Remove(clientId);
            if (updatePlayerInfo != null) //means there is another Player that will get made the new host
            {
                var handler = _playerCardHandlers[updatePlayerInfo.ClientId];
                handler.SetupServer(updatePlayerInfo);

                
                var newHostconn = ServerManager.Clients[updatePlayerInfo.ClientId];
                TargetEnablePlayButton(newHostconn);
            }
            TargetLeaveSession(conn);
        }

        [TargetRpc]
        private void TargetLeaveSession(NetworkConnection conn)
        {
            ClientManager.StopConnection();
        }

        public static void UpdateName(string newName) => _instance.HandleUpdateName(newName);

        private void HandleUpdateName(string newName)
        {
            ServerUpdateName(newName, ClientManager.Connection.ClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ServerUpdateName(string newName, int clientId)
        {
            var updatePlayerInfo = SessionDataSystem.Instance.UpdateName(newName, clientId);
            var handler = _playerCardHandlers[clientId];
            handler.SetupServer(updatePlayerInfo);
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