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
            }
            else
            {
                _pullButton.gameObject.SetActive(false);
            }
        }
        

        private void OnClickPullButton()
        {
            OnTug?.Invoke();
        }
    }
}