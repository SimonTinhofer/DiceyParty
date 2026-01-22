using UnityEngine;

namespace DiceyParty.MiniGame.QuickMath
{
    [CreateAssetMenu(fileName = "QuickMathConfig", menuName = "Scriptable Objects/QuickMath Config")]
    public class QuickMathConfigSO : ScriptableObject
    {
        public float Speed = 5f;
    }
}