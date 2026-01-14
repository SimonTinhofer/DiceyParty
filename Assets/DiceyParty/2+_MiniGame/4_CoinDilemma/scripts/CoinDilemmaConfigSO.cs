using UnityEngine;

namespace DiceyParty.MiniGame.CoinDilemma
{
    [CreateAssetMenu(fileName = "CoinDilemmaConfig", menuName = "Scriptable Objects/CoinDilemma Config")]
    public class CoinDilemmaConfigSO : ScriptableObject
    {
        public float RoundDecisionPhaseDuration = 10f;
        public float RoundResultPhaseDuration = 5f;
        public int RoundCount = 3;
        public float NonOwnerScoreAlpha = 0.5f;
    }
}