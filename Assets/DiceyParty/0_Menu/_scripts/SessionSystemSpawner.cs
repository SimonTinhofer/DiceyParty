using System;
using FishNet.Managing;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.Menu
{
    public class SessionSystemSpawner : NetworkBehaviour
    {
        public Session Session;
        public bool ClientIsHost;
        [SerializeField] private GameObject _sessionSystemPrefab;
        [SerializeField] private Button _startButton;
        [SerializeField] private bool _isTester;

        public override void OnStartServer()
        {
            base.OnStartServer();
            SpawnSessionSystem();
            if (!_isTester) return;
            _startButton.interactable = true;
            _startButton.onClick.AddListener(StartSessionTest);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if(_isTester || !ClientIsHost) return;
            StartSession(Session);
        }

        private void SpawnSessionSystem()
        {
            NetworkObject nob = NetworkManager.GetPooledInstantiated(_sessionSystemPrefab, true);
            NetworkManager.ServerManager.Spawn(nob);
        }
        
        [ServerRpc (RequireOwnership = false)]
        private void StartSession(Session session)
        {
            SessionDataSystem.Instance.SetSession(session);
            if (session.Name != "00")
            {
                SessionAnalyticsSystem.Instance.Setup(session.DeploymentId);
            }
            SessionAnalyticsSystem.Instance.SessionStarted();
            SessionStageSystem.ChangeState(SessionStage.Lobby);
        }

        private void StartSessionTest()
        {
            if (!IsServerInitialized) throw new Exception("method must only be called on the server");

            Session session = new()
            {
                Name = "testSession",
                DeploymentId = "testDeployment"
            };
            SessionDataSystem.Instance.SetSession(session);
            SessionAnalyticsSystem.Instance.SessionStarted();
            SessionStageSystem.ChangeState(SessionStage.Lobby);

        }
    }
}