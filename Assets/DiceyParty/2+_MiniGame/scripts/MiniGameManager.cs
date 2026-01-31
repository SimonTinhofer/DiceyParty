using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private MiniGamePhase _currentPhase = MiniGamePhase.Tutorial;

        [SerializeField] MiniGameResultsProcessor _resultsProcessor;
        [SerializeField] private GlobalConfigSO _globalConfig;
        private int _clientId;
        private bool _isReadyForGame;

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
            MiniGameWrapperUI.Instance.ToggleTutorialPanel(false);
        }

        public override void OnStartClient()
        {
            _clientId = ClientManager.Connection.ClientId;
            base.OnStartClient();
            TryTutorialPhase();
        }

        #region TutorialPhase
        
        private async void TryTutorialPhase()
        {
            try
            {
                await TutorialPhase();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Due to GO being destroyed during async operation it was canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"OnStartGamePhase loop failed: {e.Message}");
            }
        }

        private async Awaitable TutorialPhase()
        {
            MiniGameWrapperUI.Instance.ReadyButton.onClick.AddListener(ReadyForGame);
            await Awaitable.WaitForSecondsAsync(_globalConfig.TutorialDuration, destroyCancellationToken);
            ReadyForGame();
        }

        private void ReadyForGame()
        {
            if(_isReadyForGame) return;
            _isReadyForGame = true;
            MiniGameWrapperUI.Instance.ReadyButton.interactable = false;
            ReadyForGameServer(_clientId);
        }

        [ServerRpc (RequireOwnership = false)]
        private void ReadyForGameServer(int clientId)
        {
            if (_currentPhase != MiniGamePhase.Tutorial){ return;}
            
            _readyPlayers.Add(clientId);
            if (_readyPlayers.Count < _playerCount) return;
            if (_readyPlayers.Count > _playerCount){
                Debug.LogWarning($"ClientReadyForGamePhase was called too often!");
                return;
            }
            
            OnStartGamePhase?.Invoke();
            StartGamePhaseObservers();
            _readyPlayers.Clear();
        }

        #endregion

        #region GamePhase

        [ObserversRpc]
        private void StartGamePhaseObservers()
        {
            _currentPhase = MiniGamePhase.Game;
            MiniGameWrapperUI.Instance.ToggleTutorialPanel(false);
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
            _currentPhase = MiniGamePhase.Result;
            StartResultPhaseObservers(resultCardData);
        }

        #endregion

        #region ResultPhase

        [ObserversRpc]
        private void StartResultPhaseObservers(Dictionary<int, ResultCardInfo> resultCardData)
        {
            TryPlayOutResultPhase(resultCardData);
        }
        private async void TryPlayOutResultPhase(Dictionary<int, ResultCardInfo> resultCardData)
        {
            try
            {
                await PlayOutResultPhase(resultCardData);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Due to GO being destroyed during async operation it was canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"OnStartGamePhase loop failed: {e.Message}");
            }
        }

        private async Awaitable PlayOutResultPhase (Dictionary<int, ResultCardInfo> resultCardData)
        {
            MiniGameWrapperUI.Instance.ShowResultsPanel(resultCardData);
            await Awaitable.WaitForSecondsAsync(_globalConfig.ResultsDuration, destroyCancellationToken);
            ClientFinishedResultPhase(_clientId);
        }

        [ServerRpc (RequireOwnership = false)]
        private void ClientFinishedResultPhase(int clientId)
        {
            if (_currentPhase != MiniGamePhase.Result)
            {
                Debug.LogWarning($"FinishedResultPhase was called in {_currentPhase}!");
                return;
            }
            
            _readyPlayers.Add(clientId);
            if (_readyPlayers.Count < _playerCount) return;
            if (_readyPlayers.Count > _playerCount){
                Debug.LogWarning($"FinishedResultPhase was called too often!");
                return;
            }
            StartLobbyStage();
        }

        private void StartLobbyStage()
        {
            SessionStageSystem.ChangeState(SessionStage.Lobby);
        }

        #endregion

        private enum MiniGamePhase
        {
            Tutorial,
            Game,
            Result
        }
    }
}