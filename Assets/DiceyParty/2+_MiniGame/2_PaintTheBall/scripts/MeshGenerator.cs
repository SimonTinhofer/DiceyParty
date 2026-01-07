using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class MeshGenerator : MonoBehaviour
    {
        [SerializeField] private GameConfigSO _gameConfig;
        
        void OnValidate()
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            MeshCollider mc = GetComponent<MeshCollider>();
            Mesh mesh = CreateTriangleSphere(_gameConfig.IcoSpereSubdivisionLevel);
            mf.mesh = mesh;
            mc.sharedMesh = mesh;
            transform.localScale = new Vector3(_gameConfig.IcoSphereScale, _gameConfig.IcoSphereScale, _gameConfig.IcoSphereScale); 
        }

        Mesh CreateTriangleSphere(int subdivisions)
        {
            var t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

            var baseVerts = new Vector3[]
            {
            new Vector3(-1,  t,  0).normalized,
            new Vector3( 1,  t,  0).normalized,
            new Vector3(-1, -t,  0).normalized,
            new Vector3( 1, -t,  0).normalized,

            new Vector3( 0, -1,  t).normalized,
            new Vector3( 0,  1,  t).normalized,
            new Vector3( 0, -1, -t).normalized,
            new Vector3( 0,  1, -t).normalized,

            new Vector3( t,  0, -1).normalized,
            new Vector3( t,  0,  1).normalized,
            new Vector3(-t,  0, -1).normalized,
            new Vector3(-t,  0,  1).normalized
            };

            var baseTris = new int[]
            {
            0,11,5,  0,5,1,  0,1,7,  0,7,10,  0,10,11,
            1,5,9,  5,11,4, 11,10,2, 10,7,6,  7,1,8,
            3,9,4,  3,4,2,  3,2,6,  3,6,8,  3,8,9,
            4,9,5,  2,4,11, 6,2,10, 8,6,7,  9,8,1
            };

            var vertList = new List<Vector3>();
            var triList = new List<int>();

            for (int i = 0; i < baseTris.Length; i += 3)
            {
                SubdivideTriangle(baseVerts[baseTris[i]],
                                  baseVerts[baseTris[i + 1]],
                                  baseVerts[baseTris[i + 2]],
                                  subdivisions,
                                  vertList,
                                  triList);
            }

            // Separate vertices per triangle for independent coloring
            var newVerts = new Vector3[triList.Count];
            var newTris = new int[triList.Count];
            var colors = new Color[triList.Count];
            var uvs = new Vector2[triList.Count];

            for (int i = 0; i < triList.Count; i += 3)
            {
                Color triColor = Color.white;
                
                // Assign specific coordinates to each of the 3 vertices in the triangle
                uvs[i] = new Vector2(1, 0);
                uvs[i + 1] = new Vector2(0, 1);
                uvs[i + 2] = new Vector2(0, 0);

                for (int j = 0; j < 3; j++)
                {
                    int index = i + j;
                    newVerts[index] = vertList[triList[index]];
                    newTris[index] = index;
                    colors[index] = triColor;
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = newVerts;
            mesh.triangles = newTris;
            mesh.colors = colors;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        void SubdivideTriangle(Vector3 v1, Vector3 v2, Vector3 v3, int depth, List<Vector3> verts, List<int> tris)
        {
            if (depth == 0)
            {
                int indexStart = verts.Count;
                verts.Add(v1.normalized);
                verts.Add(v2.normalized);
                verts.Add(v3.normalized);
                tris.Add(indexStart);
                tris.Add(indexStart + 1);
                tris.Add(indexStart + 2);
                return;
            }

            Vector3 v12 = ((v1 + v2) * 0.5f).normalized;
            Vector3 v23 = ((v2 + v3) * 0.5f).normalized;
            Vector3 v31 = ((v3 + v1) * 0.5f).normalized;

            SubdivideTriangle(v1, v12, v31, depth - 1, verts, tris);
            SubdivideTriangle(v2, v23, v12, depth - 1, verts, tris);
            SubdivideTriangle(v3, v31, v23, depth - 1, verts, tris);
            SubdivideTriangle(v12, v23, v31, depth - 1, verts, tris);
        }
    }
}
