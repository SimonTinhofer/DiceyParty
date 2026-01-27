using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.CoinDilemma
{
    public class CoinDilemmaManager : NetworkBehaviour
    {
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private CoinDilemmaConfigSO _gameConfig;
        [SerializeField] private GameObject _playerScorePrefab;

        private Dictionary<int, PlayerScore> _playerScores = new();
        private Dictionary<int, int> _playerChoices = new();
        private Dictionary<int, int> _playerCoinAmounts = new();
        private int[] _chestAmounts;
        private int _playerCount;
        private int _currentRound;
        private int _clientId;
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            SceneManager.OnClientPresenceChangeEnd += SpawnPlayerScore;
            MiniGameManager.OnStartGamePhase += StartRound;
            foreach (var clientId in SessionDataSystem.Instance.GetClientIds())
            {
                _playerCoinAmounts.Add(clientId, 0);
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _clientId = LocalConnection.ClientId;
        }

        private void OnDestroy()
        {
            SceneManager.OnClientPresenceChangeEnd -= SpawnPlayerScore;
            MiniGameManager.OnStartGamePhase -= StartRound;
        }

        private void SpawnPlayerScore(ClientPresenceChangeEventArgs args)
        {
            if (!SessionDataSystem.Instance.GetClientIds().Contains(args.Connection.ClientId))
                return;
            NetworkConnection conn = args.Connection;
            NetworkObject nob = NetworkManager.GetPooledInstantiated(_playerScorePrefab,  true);
            NetworkManager.ServerManager.Spawn(nob, conn);
            _playerScores.Add(conn.ClientId, nob.GetComponent<PlayerScore>());
        }

        private void StartRound()
        {
            _currentRound++;
            _playerCount = SessionDataSystem.Instance.GetPlayerData().Count;
            int chestAmout = _playerCount;
            var chestContent = GenerateChestContent(chestAmout);
            StartRoundObserver(chestContent);
        }

        private int[] GenerateChestContent(int chestAmout)
        {
            _chestAmounts = new int[chestAmout];
            for(int i = 0; i < _chestAmounts.Length; i++)
            {
                _chestAmounts[i] = UnityEngine.Random.Range(1, 100);
            }
            return _chestAmounts;
        }
        
        [ObserversRpc (BufferLast = true)]
        private void StartRoundObserver(int[] chestContent)
        {
            TryTransparentPhase(chestContent);
        }

        private async void TryTransparentPhase(int[] chestContent)
        {
            try
            {
                await HandleTransparentPhase(chestContent);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Due to GO being destroyed during async operation it was canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"{this.name}.HandleGamePhase failed: {e.Message}");
            }
        }

        private async Awaitable HandleTransparentPhase(int[] chestContent)
        {
            UIManager.Instance.GenerateChests(chestContent, LocalConnection.ClientId);
            await Awaitable.WaitForSecondsAsync(_gameConfig.TransparentPhaseDuration, destroyCancellationToken);
            TryDecisionPhase();
        }
        
        public void SyncIndicator(int newChestIndex, int clientId)
        {
            SyncIndicatorServer(newChestIndex, clientId);
        }

        [ServerRpc (RequireOwnership = false)]
        private void SyncIndicatorServer(int newChestIndex, int clientId)
        {
            SyncIndicatorObserver(newChestIndex, clientId);
        }

        [ObserversRpc(BufferLast = true)]
        private void SyncIndicatorObserver(int newChestIndex, int clientId)
        {
            if(clientId == _clientId) return;
            UIManager.Instance.SyncIndicator(newChestIndex, clientId);
        }
        
        private async void TryDecisionPhase()
        {
            try
            {
                await HandleDecisionPhase();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Due to GO being destroyed during async operation it was canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"{this.name}.HandleGamePhase failed: {e.Message}");
            }
        }
        
        private async Awaitable HandleDecisionPhase()
        {
            UIManager.Instance.HideOtherIndicators();
            UIManager.Instance.StartTimer(_gameConfig.DecisionPhaseDuration);
            await Awaitable.WaitForSecondsAsync(_gameConfig.DecisionPhaseDuration, destroyCancellationToken);
            int chestIndex = UIManager.Instance.GetClientChestIndex();
            PassClientsChestIndex(_clientId, chestIndex);
        }


        [ServerRpc(RequireOwnership = false)]
        public void PassClientsChestIndex(int clientId, int chestIndex)
        {
            _playerChoices.Add(clientId, chestIndex);
            if (_playerChoices.Count == _playerCount)
            {
                ProcessChestChoices();
            }
        }

        private void ProcessChestChoices()
        {
            List<int>[] chestChoosers = new List<int>[_playerCount];
            foreach (var pair in _playerChoices)
            {
                int key = pair.Key;
                int chestIndex = pair.Value;
                if (chestChoosers[chestIndex] == null) chestChoosers[chestIndex] = new();
                chestChoosers[chestIndex].Add(key);
            }
            List<int> chestsToCrossOut = new();
            for (int i = 0; i < chestChoosers.Length; i++)
            {
                if(chestChoosers[i] == null) continue;
                if (chestChoosers[i].Count == 1)
                {
                    int clientId = chestChoosers[i][0];
                    _playerCoinAmounts[clientId] += _chestAmounts[i];
                }
                else if(chestChoosers[i].Count > 1)
                {
                    chestsToCrossOut.Add(i);
                }
            }
            foreach (var pair in _playerCoinAmounts)
            {
                _playerScores[pair.Key].SetCoinAmount(pair.Value);
            }
            TryFinishRound(chestsToCrossOut);
        }

        private async void TryFinishRound(List<int> chestsToCrossOut)
        {
            try
            {
                await FinishRound(chestsToCrossOut);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Due to GO being destroyed during async operation it was canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"{this.name}.FinishRound failed: {e.Message}");
            }
        }

        private async Awaitable FinishRound(List<int> chestsToCrossOut)
        {
            ShowChoicesObservers(_playerChoices, chestsToCrossOut);
            await Awaitable.WaitForSecondsAsync(_gameConfig.RoundResultPhaseDuration, destroyCancellationToken);
            _playerChoices.Clear();
            if(_currentRound < _gameConfig.RoundCount)
                StartRound();
            else
            {
                FinishMiniGame();
            }
        }

        private void FinishMiniGame()
        {
            var orderedCoinAmounts = _playerCoinAmounts.OrderByDescending(pair => pair.Value);
            Dictionary<int, int> placements = orderedCoinAmounts.Select(pair => new { pair.Key, Rank = orderedCoinAmounts.Count(p => p.Value > pair.Value) }).ToDictionary(pair => pair.Key, pair => pair.Rank);
            SceneManager.OnClientPresenceChangeEnd -= SpawnPlayerScore;
            MiniGameManager.FinishedGamePhase(placements);
        }

        [ObserversRpc]
        private void ShowChoicesObservers(Dictionary<int, int> playerChoices, List<int> chestsToCrossOut)
        {
            UIManager.Instance.ShowChoices(playerChoices, chestsToCrossOut);
        }
    }
}