using System;
using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Object;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceyParty.MiniGame.QuickMath
{
    public class TileManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _tileBridge;
        [SerializeField] private List<TileHandler> _tileHandler;
        private Dictionary<int, int> _tileNumbers;
        private int _calculationIndex;
        private int _numberMultiplayer;
        private float _correctResult;
        private bool _negativeNums;
        private CalculationData _currentCalculationData;
        
        public void SetupRound()
        {
            SetupRoundObserver();
        }
        
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void SetupRoundObserver()
        {
            _tileBridge.gameObject.SetActive(true);
            for(int i = 0; i < _tileHandler.Count; i++)
            {
                var handler = _tileHandler[i];
                handler.SetupTile();
            }
        }
        
        public string GenerateCalculation()
        {
            if(_calculationIndex % 5 == 0)
                _numberMultiplayer++;
            if(_calculationIndex > 5 && !_negativeNums)
                _negativeNums = true;
            
            _currentCalculationData = GenerateCalculationData(10 * _numberMultiplayer);
            _calculationIndex++;
            return _currentCalculationData.Calculation;
        }
        
        private CalculationData GenerateCalculationData(int range)
        {
            string calculation = "";
            var results = new List<float>();
            var correctResultIndex = Random.Range(0, _tileHandler.Count);
            for (var i = 0; i < _tileHandler.Count; i++)
            {
                var randomNums = GenerateRandomNums(range, _negativeNums);
                switch (_calculationIndex % 5)
                {
                    case 0:
                        if (correctResultIndex == i)
                            calculation = $"{randomNums[0]} + {randomNums[1]}";
                        results.Add(randomNums[0] + randomNums[1]);
                        break;
                    case 1:
                        if (correctResultIndex == i)
                            calculation = $"{randomNums[0]} - {randomNums[1]}";
                        results.Add(randomNums[0] - randomNums[1]);
                        break;
                    case 2:
                        if (correctResultIndex == i)
                            calculation = $"{randomNums[0]} * {randomNums[1]}";
                        results.Add(randomNums[0] * randomNums[1]);
                        break;
                    case 3:
                        if (correctResultIndex == i)
                            calculation = $"{randomNums[0]} * {randomNums[1]} + {randomNums[2]}";
                        results.Add(randomNums[0] * randomNums[1] + randomNums[2]);
                        break;
                    case 4:
                        if (correctResultIndex == i)
                            calculation = $"{randomNums[0]} - {randomNums[1]} * {randomNums[2]}";
                        results.Add(randomNums[0] - randomNums[1] * randomNums[2]);
                        break;
                    /*case 5:
                        if (correctResultIndex == i)
                            calculation = $"{randomNums[0]} + {randomNums[1]} / {randomNums[2]}";
                        results.Add(randomNums[0] + (float) randomNums[1] / randomNums[2]);
                        break;*/
                    default:
                        throw new Exception("Wrong Input");
                }
            }
            return new CalculationData(calculation, results, results[correctResultIndex]);
        }

        private int[] GenerateRandomNums(int range, bool negativeNums)
        {
            int[] randomNums = new int[3];
            for (int i = 0; i < 3; i++)
            {
                if (negativeNums)
                {
                    if(Random.value < 0.5f)
                        randomNums[i] = Random.Range(-range, 0);
                    else
                    {
                        randomNums[i] = Random.Range(1, range + 1);
                    }
                }
                else
                {
                    randomNums[i] = Random.Range(1, range + 1);
                }
            }
            return randomNums;
        }

        public void ShowResults()
        {
            ShowResultsObserver(_currentCalculationData);
        }
        
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void ShowResultsObserver(CalculationData calculationData)
        {
            _tileBridge.gameObject.SetActive(true);
            for(int i = 0; i < _tileHandler.Count; i++)
            {
                var handler = _tileHandler[i];
                handler.ShowResult(calculationData.Results[i]);
            }
            _correctResult = calculationData.CorrectResult;
        }

        public void RemoveFalseResults()
        {
            RemoveFalseResultsObserver();
        }
        
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void RemoveFalseResultsObserver()
        {
            _tileBridge.gameObject.SetActive(false);
            for(int i = 0; i < _tileHandler.Count; i++)
            {
                var handler = _tileHandler[i];
                handler.CheckResult(_correctResult);
            }
        }
    }
}