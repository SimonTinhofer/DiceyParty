using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DiceyParty.MiniGame.TugTheRope
{
    public class PullButtonHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform _pullButtonContainer;
        [SerializeField] private RectTransform _pullButtonRectTransform;
        [SerializeField] private Button _pullButton;
        private float _spawnTime;
        private float _minSize = 0.5f;
        private float _currentSize = 1f;
        private float _shrinkVelocity = 0.5f;

        private void Start()
        {
            _pullButton.onClick.AddListener(OnPullButtonClicked);
            SetButtonRandomPosition();
        }

        private void OnPullButtonClicked()
        {
            SetButtonRandomPosition();
        }

        private void SetButtonRandomPosition()
        {
            _currentSize = 1;
            _spawnTime = Time.time;
            var containerSize = _pullButtonContainer.rect.size;
            Vector2 rectPos = new(Random.Range(0, containerSize.x), Random.Range(0, containerSize.y));
            _pullButtonRectTransform.anchoredPosition = rectPos;
        }

        private void Update()
        {
            if(_currentSize <= _minSize) return;

            _currentSize -= _shrinkVelocity * Time.deltaTime;
            _pullButtonRectTransform.localScale = new (_currentSize, _currentSize, _currentSize);
        }
    }
}