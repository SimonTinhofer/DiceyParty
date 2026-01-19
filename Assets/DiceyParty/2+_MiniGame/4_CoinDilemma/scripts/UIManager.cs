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
        [SerializeField] private TMP_Text _timerText;

        private ChestController[] _chests;
        private Button[] _chestButtons;
        private int _selectedChestIndex = -1;
        private float _timerDuration;
        private float _timerStartTimeStamp;
        private bool _timerIsRunning;


        private void Awake()
        {
            if(Instance != null)
                Destroy(this.gameObject);
            else
                Instance = this;
        }

        public void GenerateChests(int[] chestAmounts, Color ownerColor)
        {
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
                chest.Initialize(chestAmounts[i], SessionDataSystem.Instance.GetPlayerData().Count, ownerColor);
            }
            int preselectedBox = Random.Range(0, chestAmounts.Length);
            ChestClicked(preselectedBox);
        }

        private void ChestClicked(int index)
        {
            if (_selectedChestIndex > -1)
            {
                int oldChest = _selectedChestIndex;
                _chests[oldChest].ToggleChoiceIndicator(false);
            }
            _selectedChestIndex = index;
            _chests[_selectedChestIndex].ToggleChoiceIndicator(true);
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
            float elapsedTime = Time.time - _timerStartTimeStamp;
            float timerTime = _timerDuration - elapsedTime;
            if (timerTime <= 0)
            {
                _timerIsRunning = false;
                _timerText.text = "00:00";
                return;
            }
            int timerSeconds = (int) MathF.Floor(timerTime);
            int timerCentiSeconds = (int) MathF.Floor((timerTime - timerSeconds)*100);
            string preSecondsZero = (timerSeconds < 10) ? "0" : "";
            string preCentiSecondsZero = (timerCentiSeconds < 10) ? "0" : "";
            _timerText.text  = $"{preSecondsZero}{timerSeconds}:{preCentiSecondsZero}{timerCentiSeconds}";
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

        public int GetChestIndex()
        {
            return _selectedChestIndex;
        }
    }
}