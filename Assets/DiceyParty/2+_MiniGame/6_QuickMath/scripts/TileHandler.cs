using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace DiceyParty.MiniGame.QuickMath
{
    public class TileHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _meshGo;
        [SerializeField] private GameObject _canvasGo;
        [SerializeField] private TMP_Text _number;
        private string _resultString;
        private bool _isEnabled = true;
        
        public void SetupTile()
        {
            _number.text = "?";
            if(!_isEnabled)
                ToggleTile(true);
        }
        
        public void ShowResult(float result)
        {
            _resultString = ResultToString(result);
            var textToShow = _resultString;
            if (_resultString[^3..] == ".00")
            {
                textToShow = _resultString[..^3];
            }
            _number.text = textToShow;
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
            _meshGo.SetActive(toggle);
            _canvasGo.SetActive(toggle);
        }

        private string ResultToString(float result)
        {
            var roundedFloat = (float)Mathf.RoundToInt(result * 100) / 100;
            var resultString = roundedFloat.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            return resultString;
        }
    }
}