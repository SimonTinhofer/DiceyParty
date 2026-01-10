using System.Collections.Generic;
using UnityEngine;

namespace DiceyParty.MiniGame.GrabABox
{
    public static class PointGenerator
    {
        public static List<Vector2> GeneratePoints(
            float minX, float maxX,
            float minY, float maxY,
            int pointCount,
            float minDistance)
        {
            List<Vector2> points = new List<Vector2>();
            int maxAttemptsPerPoint = 2000;

            for (int i = 0; i < pointCount; i++)
            {
                bool found = false;

                for (int attempt = 0; attempt < maxAttemptsPerPoint; attempt++)
                {
                    float x = Random.Range(minX, maxX);
                    float y = Random.Range(minY, maxY);
                    Vector2 candidate = new Vector2(x, y);

                    if (IsValid(candidate, points, minDistance))
                    {
                        points.Add(candidate);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Debug.LogWarning($"Failed to place out of reach {i + 1}/{pointCount}. Area too small or minDistance too large.");
                    float x = Random.Range(minX, maxX);
                    float y = Random.Range(minY, maxY);
                    Vector2 candidate = new Vector2(x, y);
                    points.Add(candidate);
                    break;
                }
            }

            return points;
        }

        private static bool IsValid(Vector2 candidate, List<Vector2> existing, float minDistance)
        {
            foreach (var p in existing)
            {
                if (Vector2.Distance(candidate, p) < minDistance)
                    return false;
            }
            return true;
        }
    }
}