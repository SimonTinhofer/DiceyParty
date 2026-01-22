using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.MiniGame
{
    public class OwnerColor : NetworkBehaviour
    {
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Image _image;
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            int colorIndex = SessionDataSystem.Instance.GetPlayerData()[OwnerId].ColorIndex;
            Color color = _globalConfig.Colors[colorIndex];
            
            if(_renderer)
                _renderer.material.color = color;
            if(_image)
                _image.color = color;
        }
    }
}