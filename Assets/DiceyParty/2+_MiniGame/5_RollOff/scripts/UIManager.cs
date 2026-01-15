using System;
using TMPro;
using UnityEngine;

namespace DiceyParty.MiniGame.RollOff
{
    public class UIManager : MonoBehaviour
    {
        private bool _timerRunning;
        private float _timerTimestamp;
        private int _prevSecondsLeft;
        private int _timerDuration;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private Transform _scoreParent;

        public static UIManager Instance;

        private void Awake()
        {
            if(Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        public Transform GetScoreParent()
        {
            return _scoreParent;
        }

        public void StartTimer(int duration)
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