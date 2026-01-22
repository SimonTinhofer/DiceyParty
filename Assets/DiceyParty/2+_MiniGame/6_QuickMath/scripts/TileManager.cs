using System;
using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Object;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceyParty.MiniGame.QuickMath
{
    public class TileManager : NetworkBehaviour
    {
        [SerializeField] private List<TileHandler> _tileHandler;
        [SerializeField] private TMP_Text _calculation;
        private Dictionary<int, int> _tileNumbers;
        private int _calcIndex;
        private int _numberMultiplayer;
        private float _correctResult;
        private bool _negativeNums;

        public override void OnStartServer()
        {
            base.OnStartServer();
            RoundCycler();
        }

        //nur zum Testen, Rounds werden im fertigen MiniGame dann von anderen QuickMathManager gestartet werden
        private async void RoundCycler()
        {
            while (true)
            {
                StartRound();
                await Awaitable.WaitForSecondsAsync(10);
                EndRoundObservers();
                await Awaitable.WaitForSecondsAsync(5);
            }
        }

        [Server]
        private void StartRound()
        {
            if(_calcIndex % 6 == 0)
                _numberMultiplayer++;
            if(_calcIndex > 5 && !_negativeNums)
                _negativeNums = true;
            
            var calculationData = GenerateRoundData(_calcIndex, 10 * _numberMultiplayer);
            SetupRoundObserver(calculationData);
            _calcIndex++;
        }
        
        private RoundData GenerateRoundData(int calcIndex, int range)
        {
            var calculation = "error";
            var results = new List<float>();
            var correctResultIndex = Random.Range(0, _tileHandler.Count);
            for (var i = 0; i < _tileHandler.Count; i++)
            {
                var randomNums = GenerateRandomNums(range, _negativeNums);
                switch (_calcIndex % 6)
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
                            calculation = $"{randomNums[0]} / {randomNums[1]}";
                        results.Add((float) randomNums[0] / randomNums[1]);
                        break;
                    case 4:
                        if (correctResultIndex == i)
                            calculation = $"{randomNums[0]} + {randomNums[1]} * {randomNums[2]}";
                        results.Add(randomNums[0] + randomNums[1] * randomNums[2]);
                        break;
                    case 5:
                        if (correctResultIndex == i)
                            calculation = $"{randomNums[0]} + {randomNums[1]} / {randomNums[2]}";
                        results.Add(randomNums[0] + (float) randomNums[1] / randomNums[2]);
                        break;
                    default:
                        throw new Exception("Wrong Input");
                }
            }
            return new RoundData(calculation, results, results[correctResultIndex]);
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

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void SetupRoundObserver(RoundData roundData)
        {
            for(int i = 0; i < _tileHandler.Count; i++)
            {
                var handler = _tileHandler[i];
                handler.SetupTile(roundData.Results[i]);
            }
            _calculation.text = roundData.Calculation;
            _correctResult = roundData.CorrectResult;
        }
        
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void EndRoundObservers()
        {
            for(int i = 0; i < _tileHandler.Count; i++)
            {
                var handler = _tileHandler[i];
                handler.CheckResult(_correctResult);
            }
        }

        public class RoundData
        {
            public readonly string Calculation;
            public readonly List<float> Results;
            public readonly float CorrectResult;

            public RoundData(string calculation, List<float> results, float correctResult)
            {
                Calculation = calculation;
                Results = results;
                CorrectResult = correctResult;
            }

            public RoundData(){}
        }
    }
}