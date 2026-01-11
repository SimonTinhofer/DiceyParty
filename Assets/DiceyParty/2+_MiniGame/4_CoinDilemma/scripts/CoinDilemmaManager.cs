using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.CoinDilemma
{
    public class CoinDilemmaManager : NetworkBehaviour
    {
        [SerializeField] private CoinDilemmaConfigSO _gameConfig;
        [SerializeField] private GameObject _playerScorePrefab;

        private Dictionary<int, PlayerScore> _playerScores = new();
        private Dictionary<int, int> _playerChoices = new();
        private Dictionary<int, int> _playerCoinAmounts = new();
        private int[] _chestAmounts;
        private int _playerCount;
        private int _currentRound;
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            SceneManager.OnClientPresenceChangeEnd += SpawnPlayerScore;
            MiniGameManager.OnStartGamePhase += StartRound;
        }

        private void OnDestroy()
        {
            SceneManager.OnClientPresenceChangeEnd -= SpawnPlayerScore;
            MiniGameManager.OnStartGamePhase -= StartRound;
        }

        private void SpawnPlayerScore(ClientPresenceChangeEventArgs args)
        {
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
                _chestAmounts[i] = UnityEngine.Random.Range(1, 7) * 5;
            }
            return _chestAmounts;
        }
        
        [ObserversRpc (BufferLast = true)]
        private void StartRoundObserver(int[] chestContent)
        {
            UIManager.Instance.GenerateChests(chestContent);
            UIManager.Instance.StartTimer(_gameConfig.RoundDecisionPhaseDuration);
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
                    if(_playerCoinAmounts.ContainsKey(clientId))
                        _playerCoinAmounts[clientId] += _chestAmounts[i];
                    else
                        _playerCoinAmounts.Add(clientId, _chestAmounts[i]);
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
            FinishRound(chestsToCrossOut);
        }

        private async void FinishRound(List<int> chestsToCrossOut)
        {
            ShowChoicesObservers(_playerChoices, chestsToCrossOut);
            await Awaitable.WaitForSecondsAsync(_gameConfig.RoundResultPhaseDuration);
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
            Dictionary<int, int> placements = orderedCoinAmounts.Select((pair, index) => new { pair.Key, Rank = index }).ToDictionary(pair => pair.Key, pair => pair.Rank);
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