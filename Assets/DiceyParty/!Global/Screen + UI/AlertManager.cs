using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty
{
    public class AlertManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textArea;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private GameObject _alertContainer;
        public static AlertManager Instance;
        public static Action OnNewAlertManagerLoaded;

        private void Awake()
        {
            Instance = this;
            OnNewAlertManagerLoaded?.Invoke();
            _confirmButton.onClick.AddListener(() => ToggleAlert(false));
        }

        private void ToggleAlert(bool toggle)
        {
            _alertContainer.SetActive(toggle);
        }

        public void CreateAlert(string msg)
        {
            _textArea.text = msg;
            ToggleAlert(true);
        }
    }
}