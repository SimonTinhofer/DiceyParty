using System.Collections.Generic;

namespace DiceyParty.MiniGame.QuickMath
{
    public class CalculationData
    {
        public readonly string Calculation;
        public readonly List<float> Results;
        public readonly float CorrectResult;

        public CalculationData(string calculation, List<float> results, float correctResult)
        {
            Calculation = calculation;
            Results = results;
            CorrectResult = correctResult;
        }

        public CalculationData(){}
    }
}