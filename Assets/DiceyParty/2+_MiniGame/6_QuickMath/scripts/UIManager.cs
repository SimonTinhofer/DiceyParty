using System;
using TMPro;
using UnityEngine;

namespace DiceyParty.MiniGame.QuickMath
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [SerializeField] private TMP_Text _calculation;
        [SerializeField] private TMP_Text _timer;
        
        private bool _timerRunning;
        private float _timerTimestamp;
        private int _timerDuration;
        private int _prevSecondsLeft;

        private void Awake()
        {
            if(Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        public void SetCalculation(string calculation)
        {
            _calculation.text = calculation;
        }
        
        public void StartTimer(int duration)
        {
            _timerRunning = true;
            _timerTimestamp = Time.time;
            _timerDuration = duration;
            _prevSecondsLeft = duration + 1;
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
            int secondsInTenths = Mathf.CeilToInt(timeLeft*10);
            
            if (_prevSecondsLeft == secondsInTenths) return;
            if (secondsInTenths <= 0)
            {
                _timerRunning = false;
                _timer.text = $"";
                return;
            }

            string timeleft = secondsInTenths.ToString();
            _timer.text = $"{timeleft[..^1]}.{timeleft[^1]}s";
            _prevSecondsLeft = secondsInTenths;
        }
        
    }
}