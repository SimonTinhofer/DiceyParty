using System;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.MiniGame.RollOff
{
    public class PlayerScoreHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text _longestRunText;
        [SerializeField] private Image _bgImage;
        [SerializeField] private GlobalConfigSO _globalConfig;

        public void Setup(PlayerScoreInfo scoreInfo, int localClientId)
        {
            Color bgColor = _globalConfig.Colors[scoreInfo.ColorIndex];
            if (scoreInfo.ClientId != localClientId)
                bgColor.a = 0.7f;
            _bgImage.color = bgColor;
        }

        public void UpdateLongestRun(int longestRun)
        {
            _longestRunText.text = longestRun + "m";
        }
    }
}