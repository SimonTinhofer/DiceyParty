using System.Collections.Generic;
using UnityEngine;

namespace DiceyParty
{
    [CreateAssetMenu(fileName = "GlobalConfigSO", menuName = "Scriptable Objects/GlobalConfig")]
    public class GlobalConfigSO : ScriptableObject
    {
        public int MaxPlayerCount = 6;
        public Color[] Colors =
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.magenta,
            Color.cyan
        };

        public float TutorialDuration = 2;
        public float ResultsDuration = 2;
        public int MaxNameLength = 10;
    }
}