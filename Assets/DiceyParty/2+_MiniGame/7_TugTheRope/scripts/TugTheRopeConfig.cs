using UnityEngine;

namespace DiceyParty.MiniGame.TugTheRope
{
    [CreateAssetMenu(fileName = "TugTheRopeConfig", menuName = "Scriptable Objects/TugTheRope Config")]
    public class TugTheRopeConfig : ScriptableObject
    {
        public float BaseStepSize = 1;
        public float StepSizeGrowth = 0.2f;
        public float Sharpness = 2;
        
    }
}