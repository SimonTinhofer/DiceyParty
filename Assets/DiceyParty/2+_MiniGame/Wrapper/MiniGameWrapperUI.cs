using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.MiniGame
{
    public class MiniGameWrapperUI : MonoBehaviour
    {
        public static MiniGameWrapperUI Instance;
        
        public Button ReadyButton;
        [SerializeField] GameObject _tutorialPanel;
        [SerializeField] GameObject _resultsPanel;
        [SerializeField] ResultsUIController _resultsUIController;
        

        private void Awake()
        {
            if(Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this;
                _tutorialPanel.SetActive(true);
                _resultsPanel.SetActive(false);
            }

        }

        public void ToggleTutorialPanel(bool toggle)
        {
            _tutorialPanel.SetActive(toggle);
        }

        public void ShowResultsPanel(Dictionary<int, ResultCardInfo> resultCardData)
        { 
            _resultsPanel.SetActive(true);
            _resultsUIController.CreateResultCards(resultCardData);
        }
    }
}