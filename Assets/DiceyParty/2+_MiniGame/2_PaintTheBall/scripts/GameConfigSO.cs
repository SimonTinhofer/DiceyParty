using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    [CreateAssetMenu(fileName = "MiniGame3Config", menuName = "Scriptable Objects/MiniGame3 Config")]
    public class GameConfigSO : ScriptableObject
    {
        public int IcoSpereSubdivisionLevel = 2;
        public float IcoSphereScale = 5;
        public float PaintRadius = 0.2f;
        public float ShootingForce = 20;
        public float ShootingCooldown = 1;
        public float MinForce = 0.3f;
        public float LookSpeed = 30;
        public float MaxYaw = 45;
        public float MaxPitch = 45;
        public float SphereDirectionChangeInterval = 2;
        public float SphereRotationSpeed = 20;
        public float GameDuration = 20;

        public float AngleSpeed = 90;
        public float Radius = 13.5f;
        public Vector3 RotationOffset = new Vector3(0, -90, 0);
    }
}

