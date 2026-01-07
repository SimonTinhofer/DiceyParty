using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class TriangleHandler : MonoBehaviour
    {
        [SerializeField] private GameConfigSO _gameConfig;
        [SerializeField] private GlobalConfigSO _globalConfig;

        private Mesh _mesh;
        private IcoTriangle[] _triangles;

        public int TrianglesCount => _triangles.Length;

        private void Start()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            CreateTriangleArray();
        }

        private void CreateTriangleArray()
        {
            _triangles = new IcoTriangle[_mesh.triangles.Length / 3];
            for (int i = 0; i < _triangles.Length; i++)
            {
                int j = i * 3;
                int[] verticeIndexes = new int[]
                {

                    _mesh.triangles[j],
                    _mesh.triangles[j + 1],
                    _mesh.triangles[j + 2]
                };
                IcoTriangle triangle = new IcoTriangle(verticeIndexes, -1, 0, i);
                _triangles[i] = triangle;
            }
        }

        public void RequestColorChange(List<IcoTriangle> requestedTriangles)
        {
            List<IcoTriangle> trianglesToColor = new();
            foreach (IcoTriangle triangle in requestedTriangles)
            {
                IcoTriangle prevTriangle = _triangles[triangle.ID];
                if (prevTriangle.HitTime > triangle.HitTime)
                    return;

                _triangles[triangle.ID] = triangle;
                trianglesToColor.Add(triangle);
            }
            if (trianglesToColor.Count > 0)
                ColorTriangles(trianglesToColor);
        }

        private void ColorTriangles(List<IcoTriangle> trianglesToColor)
        {
            var colors = _mesh.colors;
            foreach (IcoTriangle triangle in trianglesToColor)
            {
                Color color = _globalConfig.Colors[triangle.Owner];

                for(int i = 0; i < triangle.VerticeIndexes.Length; i++)
                {
                    colors[triangle.VerticeIndexes[i]] = color;
                }
            }
            _mesh.colors = colors;
        }
    }
}

