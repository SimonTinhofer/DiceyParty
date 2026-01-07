using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class IcoTriangle
    {
        public int[] VerticeIndexes;
        public int Owner;
        public float HitTime;
        public int ID;

        public IcoTriangle()
        {
        }

        public IcoTriangle(int[] verticeIndexes, int owner, float hitTime, int id)
        {
            VerticeIndexes = verticeIndexes;
            Owner = owner;
            HitTime = hitTime;
            ID = id;
        }
    }
}

