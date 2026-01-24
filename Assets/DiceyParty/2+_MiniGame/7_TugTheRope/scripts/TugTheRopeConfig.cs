using UnityEngine;

namespace DiceyParty.MiniGame.TugTheRope
{
    [CreateAssetMenu(fileName = "TugTheRopeConfig", menuName = "Scriptable Objects/TugTheRope Config")]
    public class TugTheRopeConfig : ScriptableObject
    {
        public int MassPerPlayer = 20;
        public int BaseMass = 60;
        public float TugForceIncrease = 600;
        public float TimeApplyTugForceIncrease = 1f;
        public float[] BalancingMultiplyers = {1, 1.33f, 1.67f};
    }
}