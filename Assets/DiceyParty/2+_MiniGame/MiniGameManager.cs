using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame
{
    public class MiniGameManager : NetworkBehaviour
    {
        public static Action OnStartGamePhase;

        private static MiniGameManager _instance;
        private int _playerCount;
        private readonly HashSet<int> _readyPlayers = new();
        private MiniGamePhase _currentPhase = MiniGamePhase.TutorialPhase;

        [SerializeField] MiniGameResultsProcessor _resultsProcessor;
        [SerializeField] private MiniGameWrapper _miniGameWrapper;
        private int _clientId;

        public static void FinishedGamePhase(Dictionary<int, int> placements) =>
            _instance.HandleFinishedGamePhase(placements);
        
        private void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Instance should be null");
            }
            _instance = this;
        }

        private void OnDestroy()
        {
            OnStartGamePhase = null;
        }

        public override void OnStartServer()
        {
            _playerCount = SessionDataSystem.Instance.GetPlayerData().Count;
        }

        public override void OnStartClient()
        {
            _clientId = ClientManager.Connection.ClientId;
            base.OnStartClient();
            StartTutorialPhase();
        }

        private async void StartTutorialPhase()
        {
            await _miniGameWrapper.TutorialPhase();
            ClientFinishedTutorialPhase(_clientId);
        }

        #region TutorialPhase

        [ServerRpc (RequireOwnership = false)]
        private void ClientFinishedTutorialPhase(int clientId)
        {
            if (_currentPhase != MiniGamePhase.TutorialPhase)
            {
                Debug.LogWarning($"FinishedTutorialPhase was called in {_currentPhase}!");
                return;
            }
            
            _readyPlayers.Add(clientId);
            if (_readyPlayers.Count != _playerCount) return;
            
            _currentPhase = MiniGamePhase.GamePhase;
            StartGamePhaseObservers();
            _readyPlayers.Clear();
        }

        #endregion

        #region GamePhase

        [ObserversRpc]
        private void StartGamePhaseObservers()
        {
            OnStartGamePhase?.Invoke();
        }
        
        private void HandleFinishedGamePhase(Dictionary<int, int> placements)
        {
            if (!IsServerInitialized) throw new NotImplementedException();
            ProcessResults(placements);
        }

        private void ProcessResults(Dictionary<int, int> placements)
        {
            Dictionary<int, ResultCardInfo> resultCardData = _resultsProcessor.ProcessResults(placements);
            _currentPhase = MiniGamePhase.ResultPhase;
            StartResultPhaseObservers(resultCardData);
        }

        #endregion

        #region ResultPhase

        [ObserversRpc]
        private void StartResultPhaseObservers(Dictionary<int, ResultCardInfo> resultCardData)
        {
            PlayOutResultPhase(resultCardData);
        }
        private async void PlayOutResultPhase(Dictionary<int, ResultCardInfo> resultCardData)
        {
            await _miniGameWrapper.ResultsPhase(resultCardData);
            ClientFinishedResultPhase(_clientId);
        }
        
        [ServerRpc (RequireOwnership = false)]
        private void ClientFinishedResultPhase(int clientId)
        {
            if (_currentPhase != MiniGamePhase.ResultPhase)
            {
                Debug.LogWarning($"FinishedResultPhase was called in {_currentPhase}!");
                return;
            }
            
            _readyPlayers.Add(clientId);
            if (_readyPlayers.Count != _playerCount) return;
            
            StartLobbyStage();
            _readyPlayers.Clear();
        }

        private void StartLobbyStage()
        {
            SessionStageSystem.ChangeState(SessionStage.Lobby);
        }

        #endregion

        private enum MiniGamePhase
        {
            TutorialPhase,
            GamePhase,
            ResultPhase
        }
    }
}