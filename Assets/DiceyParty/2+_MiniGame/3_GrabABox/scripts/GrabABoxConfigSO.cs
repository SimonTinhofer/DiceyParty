using UnityEngine;

namespace DiceyParty.MiniGame.GrabABox
{
    [CreateAssetMenu(fileName = "GrabABoxConfig", menuName = "Scriptable Objects/GrabABox Config")]
    public class GrabABoxConfigSO : ScriptableObject
    {
        public float[] PlayerConstraints = new float[] {-7, 7, -3, 3 };
        public float PlayerMinDistance = 0.8f;
        public float[] BoxConstraints = new float[] { -8, 8, -4, 4};
        public float BoxMinDistance = 2f;
        public float Speed = 6f;
        public float WaitForPlayerSpawnDuration = 0.2f;
        public float WaitForSesselSpawnDuration = 2.2f;
    }
}