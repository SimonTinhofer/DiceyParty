using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.MiniGame.TugTheRope
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _pullButtonText;
        [SerializeField] private Button _pullButton;

        public static UIManager Instance;
        public static Action OnTug;
        
        private int _pulls;

        private void Awake()
        {
            if(Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        public void TogglePullButton(bool toggle)
        {
            if (toggle)
            {
                _pullButton.gameObject.SetActive(true);
                _pullButton.onClick.AddListener(OnClickPullButton);
                SetPullButtonText(_pulls);
            }
            else
            {
                _pullButton.gameObject.SetActive(false);
            }
        }

        private void SetPullButtonText(int pulls)
        {
            var nextPullMod = (pulls + 1) % 5;
            if ( nextPullMod == 0)
                _pullButtonText.text = "Tug";
            else
                _pullButtonText.text = $"{((pulls + 1) % 5)}/5";
        }

        private void OnClickPullButton()
        {
            _pulls++;
            SetPullButtonText(_pulls);
            if (_pulls % 5 == 0 && _pulls > 0)
            {
                if(OnTug == null) return;
                OnTug.Invoke();
            }
        }
    }
}