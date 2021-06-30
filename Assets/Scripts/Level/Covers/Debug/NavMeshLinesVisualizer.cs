using System;
using System.Collections.Generic;
using Level.Covers.Classes.Util;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Level.Covers
{
    [ExecuteInEditMode]
    public class NavMeshLinesVisualizer : MonoBehaviour
    {
        public int linesCount;
        private float scanInterval = 5.0f;
        private float time;

        private List<Line> lines;

        private void Update()
        {
            time -= Time.deltaTime;
            if (time < scanInterval)
            {
                time = scanInterval;
                NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
                lines = TrianglesToLines(triangulation.indices, triangulation.vertices);
                linesCount = lines.Count;
            }
        }

        private List<Line> TrianglesToLines(int[] triangles, Vector3[] vertices)
        {
            List<Line> result = new List<Line>();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int vertexIndex1 = triangles[i];
                int vertexIndex2 = triangles[i + 1];
                int vertexIndex3 = triangles[i + 2];

                Vector3 point1 = vertices[vertexIndex1];
                Vector3 point2 = vertices[vertexIndex2];
                Vector3 point3 = vertices[vertexIndex3];

                Line line1 = new Line(point1, point2);
                Line line2 = new Line(point2, point3);
                Line line3 = new Line(point3, point1);

                result.Add(line1);
                result.Add(line2);
                result.Add(line3);
            }

            return result;
        }

        private void OnDrawGizmosSelected()
        {
            if (lines == null) 
                return;
            for (int i = 0; i < lines.Count; i++)
            {
                Line line = lines[i];
                Gizmos.color = ColorUtil.RandomColor(i);
                Gizmos.DrawLine(line.point1, line.point2);
            }
        }
    }
}