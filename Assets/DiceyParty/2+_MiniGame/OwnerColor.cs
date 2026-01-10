using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame
{
    public class OwnerColor : NetworkBehaviour
    {
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private Renderer _renderer;
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            int colorIndex = SessionDataSystem.Instance.GetPlayerData()[OwnerId].ColorIndex;
            Color color = _globalConfig.Colors[colorIndex];
            _renderer.material.color = color;
        }
    }
}