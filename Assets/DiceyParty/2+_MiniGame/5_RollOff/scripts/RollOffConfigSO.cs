using UnityEngine;

namespace DiceyParty.MiniGame.RollOff
{
    [CreateAssetMenu(fileName = "RollOffConfig", menuName = "Scriptable Objects/RollOff Config")]
    public class RollOffConfigSO : ScriptableObject
    {
        public int MaxLevel = 10;
        public float LateralBoundary = 11;
        public float MaxWaypointDistanceMultiplyer = 6;
        public float MinBlockWaypointDistanceMultiplayer = 2;
        public float LevelLength = 50;
        public float MovementAccelerationLogBase = 1.5f;
        public float AccelerationSecondsOffset = 1.5f;
        public float SpeedFunctionMultiplyer = 2f;
        public int GameDuration = 30;
    }
}