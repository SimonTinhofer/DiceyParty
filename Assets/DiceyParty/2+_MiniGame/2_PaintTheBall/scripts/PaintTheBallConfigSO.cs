using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    [CreateAssetMenu(fileName = "PaintTheBallConfig", menuName = "Scriptable Objects/PaintTheBall Config")]
    public class PaintTheBallConfigSO : ScriptableObject
    {
        public int IcoSpereSubdivisionLevel = 2;
        public float IcoSphereScale = 5;
        public float PaintRadius = 0.3f;
        public float ShootingForce = 20;
        public float ShootingCooldown = 0.7f;
        public float LookSpeed = 20;
        public float MaxPitch = 45;
        public int GameDuration = 20;

        public float AngleSpeed = 90;
        public float Radius = 13.5f;
        public Vector3 RotationOffset = new Vector3(0, -90, 0);
    }
}

