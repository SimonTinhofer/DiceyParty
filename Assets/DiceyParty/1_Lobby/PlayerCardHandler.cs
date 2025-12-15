using System;
using FishNet.Object;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DiceyParty.Lobby
{
    public class PlayerCardHandler : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private Image _colorImage;
        [SerializeField] private Image _cardImage;
        [SerializeField] private Color _cardOwnerColor;
        [SerializeField] private GlobalConfigSO _globalConfig;

        public override void OnStartClient()
        {
            base.OnStartClient();
            transform.SetParent(LobbyUIHandler.PlayerCardParent, false);
            if(IsOwner)
                    transform.SetAsFirstSibling();
        }

        public void SetupServer(PlayerInfo playerInfo)
        {
            if (!IsServerInitialized) throw new Exception("method must be called on Server");
            string playerNameText  = playerInfo.IsHost ? $"Name: {playerInfo.PlayerName} (Host)" : $"Name: {playerInfo.PlayerName}";
            _playerName.text = playerNameText;
            _colorImage.color = _globalConfig.Colors[playerInfo.ColorIndex];
            
            SetupObserver(playerNameText, playerInfo.ColorIndex);
        }
        
        [ObserversRpc (BufferLast = true)]
        private void SetupObserver(string playerNameText, int colorId)
        {
            _playerName.text = playerNameText;
            _colorImage.color = _globalConfig.Colors[colorId];
            if (!IsOwner) return;
            
            _cardImage.color = _cardOwnerColor;
            
        }
        
    }
}