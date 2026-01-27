using System.Collections.Generic;
using FishNet;
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
        [SerializeField] private Transform _indicatorContainer;
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private GameObject _cross;

        private Dictionary<int, GameObject> _indicator = new();
        private Dictionary<int, GameObject> _infoMarker = new();
        
        public void Initialize(int amount)
        {
            SetAmount(amount);
            var clientIds = SessionDataSystem.Instance.GetClientIds();
            SpawnIndicators(clientIds);
            SpawnMarkers(clientIds);
        }

        private void SpawnIndicators(IReadOnlyList<int> clientIds)
        {
            foreach (int clientId in clientIds)
            {
                GameObject indicator = Instantiate(_indicatorPrefab, _indicatorContainer);
                int colorIndex = SessionDataSystem.Instance.GetPlayerData()[clientId].ColorIndex;
                indicator.GetComponent<Image>().color = _globalConfig.Colors[colorIndex];
                indicator.SetActive(false);
                _indicator.Add(clientId, indicator);
            }
        }

        private void SetAmount(int amount)
        {
            _amountText.text = amount.ToString();
        }
        private void SpawnMarkers(IReadOnlyList<int> clientIds)
        {
            foreach (int clientId in clientIds)
            {
                GameObject infoMarker = Instantiate(_infoMarkerPrefab, _infoMarkerContainer);
                int colorIndex = SessionDataSystem.Instance.GetPlayerData()[clientId].ColorIndex;
                infoMarker.GetComponent<Image>().color = _globalConfig.Colors[colorIndex];
                infoMarker.SetActive(false);
                _infoMarker.Add(clientId, infoMarker);
            }
        }

        public void ToggleIndicator(int clientID, bool toggle)
        {
            _indicator[clientID].SetActive(toggle);
        }
        
        public void ToggleInfoMarker(int clientID, bool toggle)
        {
            _infoMarker[clientID].SetActive(toggle);
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