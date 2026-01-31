using System;
using FishNet.Object;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DiceyParty.Lobby
{
    public class PlayerCardHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private Image _colorImage;
        [SerializeField] private Image _cardImage;
        [SerializeField] private Color _cardOwnerColor;
        [SerializeField] private GlobalConfigSO _globalConfig;
        
        public void Setup(PlayerInfo playerInfo)
        {
            string playerNameText  = playerInfo.IsHost ? $"Name: {playerInfo.Name} (Host)" : $"Name: {playerInfo.Name}";
            _playerName.text = playerNameText;
            _colorImage.color = _globalConfig.Colors[playerInfo.ColorIndex];
        }
    }
}