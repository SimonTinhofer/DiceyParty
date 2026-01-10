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
        public string _sessionId = "empty";
        public bool _isHost;
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
            if(_isTester || !_isHost) return;
            StartSession(_sessionId);
        }

        private void SpawnSessionSystem()
        {
            NetworkObject nob = NetworkManager.GetPooledInstantiated(_sessionSystemPrefab, true);
            NetworkManager.ServerManager.Spawn(nob);
        }
        
        [ServerRpc (RequireOwnership = false)]
        private void StartSession(string sessionId)
        {
            SessionDataSystem.Instance.SetSessionId(sessionId);
            SessionStageSystem.ChangeState(SessionStage.Lobby);
        }

        private void StartSessionTest()
        {
            if (!IsServerInitialized) throw new Exception("method must only be called on the server");
            
            SessionDataSystem.Instance.SetSessionId("test");
            SessionStageSystem.ChangeState(SessionStage.Lobby);

        }
    }
}