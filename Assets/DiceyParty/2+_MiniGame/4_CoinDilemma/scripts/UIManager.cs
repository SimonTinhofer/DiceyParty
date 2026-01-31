using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Managing.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DiceyParty.MiniGame.CoinDilemma
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        public Transform PlayerScoreParent; 

        [SerializeField] private CoinDilemmaManager _coinDilemmaManager;
        [SerializeField] private GameObject _chestPrefab;
        [SerializeField] private Transform _chestContainerTransform;
        [SerializeField] private TMP_Text _timer;

        private ChestController[] _chests;
        private Button[] _chestButtons;
        private Dictionary<int, int> _lastChestIndex = new();
        private float _timerDuration;
        private float _timerStartTimeStamp;
        private bool _timerIsRunning;

        private int _localClientId;
        private bool _transparentPhaseEnded;
        private bool _decisionPhaseEnded;
        private int _prevSecondsLeft;


        private void Awake()
        {
            if(Instance != null)
                Destroy(this.gameObject);
            else
                Instance = this;
        }

        public void GenerateChests(int[] chestAmounts, int clientId)
        {
            _transparentPhaseEnded = false;
            _decisionPhaseEnded = false;
            _localClientId = clientId;
            if(_chests != null)
                foreach (var chest in _chests)
                {
                    Destroy(chest.gameObject);
                }
            
            _chests = new ChestController[chestAmounts.Length];
            for(int i = 0; i < chestAmounts.Length; i++)
            {
                GameObject go = Instantiate(_chestPrefab, _chestContainerTransform);
                ChestController chest = go.GetComponent<ChestController>();
                _chests[i] = chest;
                int index = i;
                chest.ChestButton.onClick.AddListener(() => ChestClicked(index));
                chest.Initialize(chestAmounts[i]);
            }
            int preselectedBox = Random.Range(0, chestAmounts.Length);
            ChestClicked(preselectedBox);
        }

        private void ChestClicked(int chestIndex)
        {
            if(_decisionPhaseEnded) return;
            if (_lastChestIndex.TryGetValue(_localClientId, out int lastIndex))
            {
                _chests[lastIndex].ToggleIndicator(_localClientId, false);
            }
            _chests[chestIndex].ToggleIndicator(_localClientId, true);
            _coinDilemmaManager.SyncIndicator(chestIndex, _localClientId);
            _lastChestIndex[_localClientId] = chestIndex;
        }

        public void SyncIndicator(int chestIndex, int clientId)
        {
            if(_transparentPhaseEnded) return;
            if (_lastChestIndex.TryGetValue(clientId, out int lastIndex))
            {
                _chests[lastIndex].ToggleIndicator(clientId, false);
            }
            _chests[chestIndex].ToggleIndicator(clientId, true);
            _lastChestIndex[clientId] = chestIndex;
        }
        
        public void HideOtherIndicators()
        {
            _transparentPhaseEnded = true;
            foreach (var entry in _lastChestIndex)
            {
                if(entry.Key == _localClientId) continue;
                _chests[entry.Value].ToggleIndicator(entry.Key, false);
            }
        }

        public void StartTimer(float durationInSeconds)
        {
            _timerDuration = durationInSeconds;
            _timerStartTimeStamp = Time.time;
            _timerIsRunning = true;
        }

        private void Update()
        {
            if(_timerIsRunning)
                UpdateTimer();
        }

        private void UpdateTimer()
        {
            float timePassed = Time.time - _timerStartTimeStamp;
            float timeLeft = _timerDuration - timePassed;
            int secondsInTenths = Mathf.CeilToInt(timeLeft*10);
            
            if (_prevSecondsLeft == secondsInTenths) return;
            if (secondsInTenths <= 0)
            {
                _timerIsRunning = false;
                _timer.text = $"";
                return;
            }

            string timeleft = secondsInTenths.ToString();
            _timer.text = $"{timeleft[..^1]}.{timeleft[^1]}s";
            _prevSecondsLeft = secondsInTenths;
        }

        public void ShowChoices(Dictionary<int, int> playerChoices, List<int> chestsToCrossOut)
        {
            foreach (var pair in playerChoices)
            {
                _chests[pair.Value].ToggleInfoMarker(pair.Key, true);
            }
            foreach (var index in chestsToCrossOut)
            {
                _chests[index].ToggleCross(true);
            }
        }

        public int GetClientChestIndex()
        {
            _decisionPhaseEnded = true;
            return _lastChestIndex[_localClientId];
        }
    }
}