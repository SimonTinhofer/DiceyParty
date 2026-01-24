using System;
using UnityEngine;

namespace DiceyParty.MiniGame
{
    public class TextBgResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform _textTransform;
        [SerializeField] private RectTransform _bgTransfrom;
        private Vector2 _padding = new (25, 15);
        private Vector2 _textSize;

        private void Update()
        {
            if (_textSize != _textTransform.rect.size)
            {
                Resize(); 
            }
        }

        private void Resize()
        {
            _textSize = _textTransform.rect.size;
            _bgTransfrom.sizeDelta = _textSize + _padding;
        }
    }
}