using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.MiniGame.CoinDilemma
{
    public class ChestController : MonoBehaviour
    {
        [SerializeField] public Button ChestButton;

        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Transform _infoMarkerContainer;
        [SerializeField] private GameObject _infoMarkerPrefab;
        [SerializeField] private GameObject _choiceIndicator;
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private GameObject _cross;

        private Dictionary<int, GameObject> _infoMarkerArray = new();


        public void Initialize(int amount, int clientCount)
        {
            SetAmount(amount);
            SpawnMarkers(clientCount);
        }

        private void SetAmount(int amount)
        {
            _amountText.text = amount.ToString();
        }
        private void SpawnMarkers(int clientCount)
        {
            var clientIds = SessionDataSystem.Instance.GetClientIds();
            _infoMarkerArray = new();
            foreach (int clientId in clientIds)
            {
                GameObject infoMarker = Instantiate(_infoMarkerPrefab, _infoMarkerContainer);
                int colorIndex = SessionDataSystem.Instance.GetPlayerData()[clientId].ColorIndex;
                infoMarker.GetComponent<Image>().color = _globalConfig.Colors[colorIndex];
                infoMarker.SetActive(false);
                _infoMarkerArray.Add(clientId, infoMarker);
            }
        }

        public void ToggleInfoMarker(int clientID, bool toggle)
        {
            _infoMarkerArray[clientID].SetActive(toggle);
        }

        public void ToggleChoiceIndicator(bool toggle)
        {
            _choiceIndicator.gameObject.SetActive(toggle);
        }

        public void ToggleCross(bool toggle)
        {
            _cross.SetActive(toggle);
        }

        public void ToggleInfoMarkerContainer(bool toggle)
        {
            _infoMarkerContainer.gameObject.SetActive(toggle);
        }
    }
}