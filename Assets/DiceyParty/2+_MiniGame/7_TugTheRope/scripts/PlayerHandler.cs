using System;
using TMPro;
using UnityEngine;

namespace DiceyParty.MiniGame.TugTheRope
{
    public class PlayerHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameTag;
        [SerializeField] private TMP_Text _tugsText;
        
        [SerializeField] private Renderer _renderer;
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private Transform _bodyTransform;

        public void Setup(string playerName, int colorIndex, Quaternion bodyRotation, float multiplyer)
        {
            _nameTag.text = playerName;
            _renderer.material.color = _globalConfig.Colors[colorIndex];
            _bodyTransform.rotation = bodyRotation;
            if (multiplyer < 1.1) return;
            var scaleMultiplayer = 1 + (multiplyer - 1) / 2;
            transform.localScale = new Vector3(scaleMultiplayer, scaleMultiplayer, scaleMultiplayer);
        }

        public void UpdateTugsText(int tugs)
        {
            _tugsText.text = $"Tugs: {tugs}";
        }
    }
}