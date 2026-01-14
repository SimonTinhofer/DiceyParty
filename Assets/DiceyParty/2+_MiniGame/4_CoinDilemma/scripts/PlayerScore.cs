using System;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace DiceyParty.MiniGame.CoinDilemma
{
    public class PlayerScore : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _coinText;
        [SerializeField] private Image _bgImage;
        [SerializeField] private Image _coinImage;
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private CoinDilemmaConfigSO _gameConfig;
        
        private void Start()
        {
            transform.SetParent(UIManager.Instance.PlayerScoreParent, false);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            int colorIndex = SessionDataSystem.Instance.GetPlayerData()[OwnerId].ColorIndex;
            Color bgColor = _globalConfig.Colors[colorIndex];
            if (!IsOwner)
            {
                bgColor.a = _gameConfig.NonOwnerScoreAlpha;
                Color coinColor = _coinImage.color;
                coinColor.a = _gameConfig.NonOwnerScoreAlpha;
                _coinImage.color = coinColor;
            }
            _bgImage.color = bgColor;
        }

        public void SetCoinAmount(int newAmount)
        {
            SetCoinAmountObserver(newAmount);
        }

        [ObserversRpc]
        private void SetCoinAmountObserver(int newAmount)
        {
            _coinText.text = newAmount.ToString();
        }
    }
}