using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.MiniGame.RollOff
{
    public class PlayerScoreHandler : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _longestRunText;
        [SerializeField] private Image _bgImage;
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private Transform _scoreTransform;
        
        private void Start()
        {
            _scoreTransform.SetParent(UIManager.Instance.GetScoreParent(), false);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            int colorIndex = SessionDataSystem.Instance.GetPlayerData()[OwnerId].ColorIndex;
            Color bgColor = _globalConfig.Colors[colorIndex];
            if (!IsOwner)
                bgColor.a = 0.7f;
            _bgImage.color = bgColor;
        }

        [ServerRpc]
        public void SetLongestRun(int newAmount)
        {
            SetLongestRunObserver(newAmount);
        }

        [ObserversRpc]
        private void SetLongestRunObserver(int newAmount)
        {
            _longestRunText.text = $"{newAmount}m";
        }
    }
}