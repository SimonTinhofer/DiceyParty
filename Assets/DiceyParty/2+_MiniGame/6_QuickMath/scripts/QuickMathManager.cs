using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.QuickMath
{
    public class QuickMathManager : NetworkBehaviour
    {
        public static QuickMathManager Instance;

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private QuickMathConfigSO _gameConfig;
        [SerializeField] private TileManager _tileManager;

        private int _spawnPointIndex;
        private int _currentRound;
        private Dictionary<int, int> _results = new();
        private bool _gamePhaseHasEnded;

        private void Awake()
        {
            if(Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SceneManager.OnClientPresenceChangeEnd += SpawnPlayer;
            MiniGameManager.OnStartGamePhase += StartGame;
        }

        private void OnDestroy()
        {
            SceneManager.OnClientPresenceChangeEnd -= SpawnPlayer;
            MiniGameManager.OnStartGamePhase -= StartGame;
        }
        
        private void SpawnPlayer(ClientPresenceChangeEventArgs args)
        {
            if (!SessionDataSystem.Instance.GetClientIds().Contains(args.Connection.ClientId)) return;
            NetworkConnection conn = args.Connection;
            NetworkObject nob = NetworkManager.GetPooledInstantiated(_playerPrefab, _spawnPoints[_spawnPointIndex % _spawnPoints.Length].position, Quaternion.identity, true);
            NetworkManager.ServerManager.Spawn(nob, conn);
            _spawnPointIndex++;
        }
        
        private void StartGame()
        {
            TryPlayOutRound();
        }

        private async void TryPlayOutRound()
        {
            try
            {
                while (!_gamePhaseHasEnded)
                {
                    await PlayOutRound();
                }
            }

            catch (OperationCanceledException)
            {
                Debug.Log($"Due to GO being destroyed during async operation it was canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"PlayOutRound failed: {e.Message}");
            }
        }

        private async Awaitable PlayOutRound()
        {
            _currentRound++;
            _tileManager.SetupRound();
            await Awaitable.WaitForSecondsAsync(_gameConfig.ShowCalculationDelay, destroyCancellationToken);
            string calculation = _tileManager.GenerateCalculation();
            SetCalculationUIObservers(calculation);
            StartTimerUIObservers(_gameConfig.ShowResultsDelay + _gameConfig.RemoveFalseTilesDelay);
            await Awaitable.WaitForSecondsAsync(_gameConfig.ShowResultsDelay, destroyCancellationToken);
            _tileManager.ShowResults();
            await Awaitable.WaitForSecondsAsync(_gameConfig.RemoveFalseTilesDelay, destroyCancellationToken);
            _tileManager.RemoveFalseResults();
            await Awaitable.WaitForSecondsAsync(_gameConfig.StartNewRoundDelay, destroyCancellationToken);
        }
        
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void StartTimerUIObservers(int i)
        {
            UIManager.Instance.StartTimer(i);
        }
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void SetCalculationUIObservers(string s)
        {
            UIManager.Instance.SetCalculation(s);
        }

        public void PlayerDied(int ownerId)
        {
            if(!_results.TryAdd(ownerId, _currentRound)) return;
            if (_results.Count == SessionDataSystem.Instance.GetClientIds().Count)
            {
                _gamePhaseHasEnded = true;
                GeneratePlacements();
            }
        }

        private void GeneratePlacements()
        {
            var orderedResults = _results.OrderByDescending(pair => pair.Value);
            Dictionary<int, int> placements = orderedResults.Select(pair => new { ClientId = pair.Key, Rank = orderedResults.Count(p => p.Value > pair.Value) }).ToDictionary(e => e.ClientId, e => e.Rank);
            SceneManager.OnClientPresenceChangeEnd -= SpawnPlayer;
            MiniGameManager.FinishedGamePhase(placements);
        }
    }
}