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
        
        private void Start()
        {
            transform.SetParent(UIManager.Instance.PlayerScoreParent, false);
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