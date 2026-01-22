using TMPro;
using UnityEngine;

namespace DiceyParty.MiniGame.QuickMath
{
    public class TileHandler : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private TMP_Text _number;
        private string _resultString;
        private bool _isEnabled = true;
        
        public void SetupTile(float result)
        {
            _resultString = ResultToString(result);
            _number.text = _resultString;
            if(!_isEnabled)
                ToggleTile(true);
        }

        public void CheckResult(float correctResult)
        {
            var isCorrect = _resultString == ResultToString(correctResult);
            if(!isCorrect)
                ToggleTile(false);
        }

        private void ToggleTile(bool toggle)
        {
            _isEnabled = toggle;
            _collider.enabled = toggle;
            _renderer.enabled = toggle;
        }

        private string ResultToString(float result)
        {
            var roundedFloat = (float)Mathf.RoundToInt(result * 100) / 100;
            var resultString = roundedFloat.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            return resultString;
        }
    }
}