using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardComponentController : MonoBehaviour
{
    [SerializeField] private Image _img;
    [SerializeField] private RectTransform _rectTransform;

    public void SetColor(Color color)
    {
        _img.color = color;
    }

    public void SetWidth(float newWidth)
    {
        _rectTransform.sizeDelta = new Vector2(newWidth, _rectTransform.sizeDelta.y);
    }
}
