using System;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class UIManager : NetworkBehaviour
    {
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private RectTransform _scoreBoardTransform;
        [SerializeField] private GameObject _scoreBoardComponentPrefab;
        [SerializeField] private TriangleHandler _triangleHandler;
        [SerializeField] private TMP_Text _timerText;
        
        private int _trianglesCount;
        private float _scorebarLength;
        private Dictionary<int, ScoreboardComponentController> _scoreControllers = new();
        private bool _timerRunning;
        private float _timerTimestamp;
        private int _timerDuration;
        private int _prevSecondsLeft;

        private static UIManager _instance;

        private void Awake()
        {
            if (_instance != null)
                throw new Exception("Instance should be null");
            _instance = this;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _trianglesCount = _triangleHandler.TriangleCount;
            _scorebarLength = _scoreBoardTransform.sizeDelta.x;
        }


        
        private void CreateScoreboardComponents(int clientId)
        {
            var s = Instantiate(_scoreBoardComponentPrefab, _scoreBoardTransform).GetComponent<ScoreboardComponentController>();
            int colorIndex = SessionDataSystem.Instance.GetPlayerData()[clientId].ColorIndex;
            s.SetColor(_globalConfig.Colors[colorIndex]);
            _scoreControllers.Add(clientId, s);
        }

        public static void UpdateScoreboard(Dictionary<int, int> playerTriangleCount) => _instance.HandleUpdateScoreBoard(playerTriangleCount);


        private void HandleUpdateScoreBoard(Dictionary<int, int> playerTriangleCount)
        {
            List<(int, float)> componentWidths = new();
            foreach(var entry in playerTriangleCount)
            {
                float triangleRatio = (float)entry.Value / _trianglesCount;
                float newWidth = _scorebarLength * triangleRatio;
                componentWidths.Add((entry.Key, newWidth));
            }
            componentWidths.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            for(int i = 0; i < componentWidths.Count; i++)
            {
                int clientId = componentWidths[i].Item1;

                if (!_scoreControllers.ContainsKey(clientId))
                {
                    CreateScoreboardComponents(clientId);
                }
                
                _scoreControllers[clientId].SetWidth(componentWidths[i].Item2);
                _scoreControllers[clientId].transform.SetAsFirstSibling();
            }
        }

        public static void StartTimer(int duration) => _instance.HandleStartTimer(duration);

        private void HandleStartTimer(int duration)
        {
            _timerRunning = true;
            _timerTimestamp = Time.time;
            _timerDuration = duration;
            _prevSecondsLeft = duration;
        }

        private void Update()
        {
            if(_timerRunning)
                UpdateTimer();
        }

        private void UpdateTimer()
        {
            float timePassed = Time.time - _timerTimestamp;
            float timeLeft = _timerDuration - timePassed;
            int secondsLeft = Mathf.CeilToInt(timeLeft);
            
            if (_prevSecondsLeft == secondsLeft) return;
            if (secondsLeft <= 0)
            {
                _timerRunning = false;
                _timerText.text = $"Time left: 0s";
            }
            
            _timerText.text = $"Time left: {secondsLeft}s";
            _prevSecondsLeft = secondsLeft;
        }
    }
}