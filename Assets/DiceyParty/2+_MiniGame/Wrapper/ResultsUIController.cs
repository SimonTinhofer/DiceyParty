using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace DiceyParty.MiniGame
{
    public class ResultsUIController : MonoBehaviour
    {
        [SerializeField] private Transform _resultCardContainer;
        [SerializeField] private GameObject _resultCardPrefab;

        public void CreateResultCards(Dictionary<int, ResultCardInfo> ResultPhaseData)
        {
            List<ResultCardController> controllers = new();
            foreach (var entry in ResultPhaseData) 
            {
                ResultCardController controller = Instantiate(_resultCardPrefab, _resultCardContainer).GetComponent<ResultCardController>();
                controller.Setup(entry.Value, entry.Key);
                controllers.Add(controller);
            }
            foreach(ResultCardController controller in controllers)
            {
                controller.SetIndex();
            }
        }
    }
}