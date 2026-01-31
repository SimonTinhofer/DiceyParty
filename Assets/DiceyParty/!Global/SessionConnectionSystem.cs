using System;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Authenticating;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Transporting;
using JetBrains.Annotations;
using UnityEngine;

namespace DiceyParty
{
    public class SessionConnectionSystem : NetworkBehaviour
    {
        [SerializeField] private GlobalConfigSO _globalConfig;
        [CanBeNull] private NetworkConnection _dcConn;
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
            SessionDataSystem.OnLastPlayerRemoved += OnLastClientDisconnected;
        }
        
        private void OnDestroy()
        {
            ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
            SessionDataSystem.OnLastPlayerRemoved -= OnLastClientDisconnected;
        }

        private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
        {
            if(args.ConnectionState == RemoteConnectionState.Started)
                OnClientConnects(conn);
            else if(args.ConnectionState == RemoteConnectionState.Stopped)
                OnClientDisconnects(conn);
                
        }

        private void OnClientConnects(NetworkConnection conn)
        {
            if (AllowConnecting()) return;
            
            _dcConn = conn;
            conn.Disconnect(true);
        }

        private void OnClientDisconnects(NetworkConnection conn)
        {
            if (conn == _dcConn)
            {
                _dcConn = null;
                return;
            }
            
            SessionDataSystem.Instance.TryRemovePlayerInfo(conn.ClientId);
            
            if(SessionStageSystem.GetCurrentStage() == SessionStage.MiniGame)
            {
                ShowAlertObservers(conn.ClientId);
                SessionStageSystem.ChangeState(SessionStage.Lobby);
            }
        }

        [ObserversRpc]
        private void ShowAlertObservers(int excludeClientId)
        {
            if(LocalConnection.ClientId == excludeClientId) return;

            AlertManager.OnNewAlertManagerLoaded += ShowStopMiniGameAlert;
        }

        private void ShowStopMiniGameAlert()
        {
            AlertManager.OnNewAlertManagerLoaded -= ShowStopMiniGameAlert;
            AlertManager.Instance.CreateAlert("MiniGame was stopped because a Player left the session during the MiniGame");
        }

        private bool AllowConnecting()
        {
            bool sessionFull = ServerManager.Clients.Count > _globalConfig.MaxPlayerCount;
            bool isLobbyStage = SessionStageSystem.GetCurrentStage() == SessionStage.Lobby;
            if (!sessionFull && isLobbyStage)
                return true;
            return false;
        }
        
        private void OnLastClientDisconnected()
        {
            Session session = SessionDataSystem.Instance.GetSession();
            if(session.DeploymentId != "test")
                TryDeleteSession(session);
            else
            {
                ServerManager.StopConnection(false);
            }
        }

        private async void TryDeleteSession(Session session)
        {
            try
            {
                await BackendAPI.DeleteSessions(session);
            }
            catch (Exception e)
            {
                Debug.Log("Error trying to delete Session: " + e);
            }
        }
    }
}