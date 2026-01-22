using UnityEngine;

namespace DiceyParty.MiniGame.QuickMath
{
    [CreateAssetMenu(fileName = "QuickMathConfig", menuName = "Scriptable Objects/QuickMath Config")]
    public class QuickMathConfigSO : ScriptableObject
    {
        public float Speed = 5f;
        public int ShowCalculationDelay = 1;
        public int ShowResultsDelay = 3;
        public int RemoveFalseTilesDelay = 4;
        public int StartNewRoundDelay = 3;
    }
}