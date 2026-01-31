using System;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace DiceyParty.MiniGame.CoinDilemma
{
    public class PlayerScoreHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinText;
        [SerializeField] private Image _bgImage;
        [SerializeField] private Image _coinImage;
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private CoinDilemmaConfigSO _gameConfig;

        public void Setup(PlayerScoreInfo scoreInfo, int localClientId)
        {
            Color bgColor = _globalConfig.Colors[scoreInfo.ColorIndex];
            if(scoreInfo.ClientId != localClientId)
            {
                bgColor.a = _gameConfig.NonOwnerScoreAlpha;
                Color coinColor = _coinImage.color;
                coinColor.a = _gameConfig.NonOwnerScoreAlpha;
                _coinImage.color = coinColor;
            }
            _bgImage.color = bgColor;
            _coinText.text = scoreInfo.CoinAmount.ToString();
        }

        public void UpdateCoinAmount(int coinAmount)
        {
            _coinText.text = coinAmount.ToString();
        }
    }
}