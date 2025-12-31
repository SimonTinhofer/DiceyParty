using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DiceyParty.MiniGame
{
    public class MiniGameWrapper : MonoBehaviour
    {
        [SerializeField] GameObject _tutorialPanel;
        [SerializeField] GameObject _resultsPanel;
        [SerializeField] ResultsUIController _resultsUIController;
        [SerializeField] private GlobalConfigSO _globalConfig;

        private void Awake()
        {
            _tutorialPanel.SetActive(true);
            _resultsPanel.SetActive(false);
        }

        public async Awaitable TutorialPhase()
        {
            _tutorialPanel.SetActive(true);
            await Awaitable.WaitForSecondsAsync(_globalConfig.TutorialDuration);
            _tutorialPanel.SetActive(false);
        }

        public async Task ResultsPhase(Dictionary<int, ResultCardInfo> ResultCardData)
        {
            _resultsPanel.SetActive(true);
            _resultsUIController.CreateResultCards(ResultCardData);
            await Awaitable.WaitForSecondsAsync(_globalConfig.ResultsDuration);
            _resultsPanel.SetActive(false);
        }
    }
}