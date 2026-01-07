using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class ProjectileHitDetection : MonoBehaviour
    {
        private Mesh _mesh;
        private Vector3[] _vertices;
        private int[] _triangles;

        void Start()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            _vertices = _mesh.vertices;
            _triangles = _mesh.triangles;
        }

        public void DetectHitTriangles(Vector3 worldPoint, float radius)
        {
            List<IcoTriangle> hitTriangles = new();

            Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
            float sqrRadius = radius * radius;
            for (int i = 0; i < _triangles.Length; i += 3)
            {
                int i0 = _triangles[i];
                int i1 = _triangles[i + 1];
                int i2 = _triangles[i + 2];

                Vector3 v0 = _vertices[i0];
                Vector3 v1 = _vertices[i1];
                Vector3 v2 = _vertices[i2];

                // Mittelpunkt des Dreiecks berechnen
                Vector3 center = (v0 + v1 + v2) / 3f;

                if ((center - localPoint).sqrMagnitude <= sqrRadius)
                {
                    hitTriangles.Add(new IcoTriangle(new int[] {i0, i1, i2}, -1, 0 , i/3));
                }
            }

            TriangleManager.LocalTrianglesHitClient(hitTriangles);
        }
    }
}

