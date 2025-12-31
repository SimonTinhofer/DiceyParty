using DiceyParty;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.MiniGame
{
    public class ResultCardController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _placement;
        [SerializeField] private GlobalConfigSO _globalConfig;

        private ResultCardInfo _resultCardInfo;

        public void Setup(ResultCardInfo resultCardInfo, int clientID)
        {
            _resultCardInfo = resultCardInfo;
            _name.text = resultCardInfo.Name;
            _image.color = _globalConfig.Colors[clientID];
            _placement.text = $"{resultCardInfo.Placement + 1}.";
        }

        public void SetIndex()
        {
            transform.SetSiblingIndex(_resultCardInfo.Placement);
        }
    }
}