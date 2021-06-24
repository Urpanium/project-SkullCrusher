using System;
using System.Collections.Generic;
using Level.Covers.Util;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Level.Covers
{
    public class CoverBaker
    {
        private readonly LayerMask shootableMask;

        private readonly float characterStandHeight;
        private readonly float characterCrouchHeight;
        private readonly float characterRadius;

        private static readonly float linePointsEpsilon = 0.01f;

        private int progressId;



        public CoverBaker(LayerMask shootableMask, float characterStandHeight, float characterCrouchHeight,
            float characterRadius)
        {
            this.shootableMask = shootableMask;
            this.characterStandHeight = characterStandHeight;
            this.characterCrouchHeight = characterCrouchHeight;
            this.characterRadius = characterRadius;
        }


        public List<Cover> BakeCovers()
        {
            progressId = Progress.Start("Baking covers", "Finding spots to hide for AI agents...");
            
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            Vector3[] vertices = triangulation.vertices;
            int[] triangles = triangulation.indices;

            // STEP 1
            List<Line> allLines = TrianglesToLines(triangles, vertices);
            // STEP 2
            List<Line> uniqueLines = GetUniqueLines(allLines);
            // STEP 3
            List<Line> connectedLines = GetConnectedLines(uniqueLines);

            /*List<Line> mergedLines = connectedLines;

            int previousCount = -1;
            int iterations = maximumLineMergeIterations;
            while (mergedLines.Count != previousCount && iterations > 0)
            {
                previousCount = mergedLines.Count;
                iterations--;
                mergedLines = GetConnectedLines(MergeLines(mergedLines));
                print(
                    $"CoverGenerator: iteration #{maximumLineMergeIterations - iterations}: {mergedLines.Count} lines");
            }

            List<Line> lines = mergedLines;*/
            // STEP 4
            List<(Vector3, Vector3)> possibleCovers = GetPossibleCoversPositionsAndDirections(connectedLines);
            // STEP 5
            List<Cover> covers = GetCovers(possibleCovers);
            Progress.Remove(progressId);
            return covers;
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


        private List<Line> GetUniqueLines(List<Line> lines)
        {
            List<Line> uniqueLines = new List<Line>();

            for (int i = 0; i < lines.Count; i++)
            {
                Line line1 = lines[i];
                bool unique = true;
                for (int j = 0; j < lines.Count; j++)
                {
                    if (i == j)
                        continue;
                    Line line2 = lines[j];
                    if (line1.Equals(line2, linePointsEpsilon))
                    {
                        unique = false;
                        break;
                    }
                }

                if (unique)
                {
                    uniqueLines.Add(line1);
                }
            }

            return uniqueLines;
        }

        private List<Line> GetConnectedLines(List<Line> lines)
        {
            List<Line> result = new List<Line>();
            for (int i = 0; i < lines.Count; i++)
            {
                Line line1 = lines[i];
                for (int j = 0; j < lines.Count; j++)
                {
                    if (i == j)
                        continue;

                    Line line2 = lines[j];
                    if (line1.HasSamePoints(line2))
                    {
                        result.Add(line1);
                        break;
                    }
                }
            }

            return result;
        }

        private bool TryMergeLines(Line line1, Line line2, out Line newLine)
        {
            if (line1.HasSamePoints(line2))
            {
                (Vector3, Vector3, Vector3) points = line1.GetSameAndDifferentPoints(line2);

                Vector3 direction1 = points.Item1 - points.Item2;
                Vector3 direction2 = points.Item1 - points.Item3;
                float dot = Vector3.Dot(direction1, direction2);
                
                if (Mathf.Abs(dot) > 0.95f)
                {
                    newLine = new Line(points.Item2, points.Item3);
                    return true;
                }
            }
            throw new NotImplementedException();
        }


        private List<(Vector3, Vector3)> GetPossibleCoversPositionsAndDirections(List<Line> uniqueLines)
        {
            Progress.Report(progressId, 0.0f);
            List<(Vector3, Vector3)> result = new List<(Vector3, Vector3)>();

            for (int i = 0; i < uniqueLines.Count; i++)
            {
                Progress.Report(progressId, 0.5f * ((i + 1.0f) / uniqueLines.Count));

                Line line = uniqueLines[i];
                Vector3 delta = line.point2 - line.point1;
                float length = delta.magnitude;
                if (length < characterRadius * 2.0f)
                {
                    Vector3 middle = Vector3.Lerp(line.point1, line.point2, 0.5f);

                    // normalize vector using already calculated length
                    Vector3 normal = GetHorizontalLineNormal(delta / length);
                    result.Add((middle, normal));
                    result.Add((middle, -normal));
                }
                else
                {
                    Vector3 lineDirection = (line.point2 - line.point1).normalized;
                    Vector3 normal = GetHorizontalLineNormal(delta / length);

                    float steps = length / (characterRadius * 2.0f);

                    for (int j = 0; j < steps; j++)
                    {
                        Vector3 offset = lineDirection * (j * characterRadius * 2.0f);
                        Vector3 position = line.point1 + offset;

                        result.Add((position, normal));
                        result.Add((position, -normal));
                    }

                    result.Add((line.point2, normal));
                    result.Add((line.point2, -normal));
                }
            }

            return result;
        }

        private List<Cover> GetCovers(List<(Vector3, Vector3)> possibleCovers)
        {
            Progress.Report(progressId, 0.5f);
            List<Cover> result = new List<Cover>();
            for (int i = 0; i < possibleCovers.Count; i++)
            {
                // (position, direction)
                Progress.Report(progressId, 0.5f + 0.5f * ((i + 1.0f) / possibleCovers.Count));
                (Vector3, Vector3) possibleCover = possibleCovers[i];
                if (TryMakeCover(possibleCover.Item1, possibleCover.Item2, out Cover cover))
                {
                    result.Add(cover);
                }
            }

            return result;
        }


        private bool TryMakeCover(Vector3 positionOnNavMesh, Vector3 calculatedNormal,
            out Cover cover)
        {
            float height = GetCoverHeight(positionOnNavMesh, calculatedNormal);

            if (height < characterCrouchHeight)
            {
                cover = null;
                return false;
            }

            CoverType type = CoverType.Stand;
            if (height < characterStandHeight)
            {
                type = CoverType.Crouch;
            }

            cover = new Cover(positionOnNavMesh, calculatedNormal, type);
            return true;
        }

        private Vector3 GetHorizontalLineNormal(Vector3 direction)
        {
            return new Vector3(direction.z, 0, -direction.x);
        }

        private float GetCoverHeight(Vector3 positionOnNavMesh, Vector3 direction)
        {
            float r2 = characterRadius * 2.0f;
            Vector3 standOffset = Vector3.up * characterStandHeight;

            Vector3 side = GetHorizontalLineNormal(direction);


            Ray standRay = new Ray(positionOnNavMesh + standOffset, direction);

            Ray rightWallRay =
                new Ray(positionOnNavMesh + standOffset + side * characterRadius,
                    direction);
            Ray leftWallRay =
                new Ray(positionOnNavMesh + standOffset - side * characterRadius,
                    direction);

            /*
            UnityEngine.Debug.DrawRay(standRay.origin, standRay.direction);
            UnityEngine.Debug.DrawRay(crouchRay.origin, crouchRay.direction);*/
            if (Physics.Raycast(standRay, r2, shootableMask)
                && (!Physics.Raycast(rightWallRay, r2, shootableMask)
                    || !Physics.Raycast(leftWallRay, r2, shootableMask)))
                return characterStandHeight;

            Vector3 crouchOffset = Vector3.up * characterCrouchHeight;

            Ray crouchRay = new Ray(positionOnNavMesh + crouchOffset, direction);
            Ray rightCrouchWallRay =
                new Ray(positionOnNavMesh + crouchOffset + side * characterRadius,
                    direction);
            Ray leftCrouchWallRay =
                new Ray(positionOnNavMesh + crouchOffset - side * characterRadius,
                    direction);

            if (
                Physics.Raycast(crouchRay, r2, shootableMask)
                && (!Physics.Raycast(rightCrouchWallRay, r2, shootableMask)
                    || !Physics.Raycast(leftCrouchWallRay, r2, shootableMask)
                    || !Physics.Raycast(standRay, r2, shootableMask)
                    || !Physics.Raycast(rightWallRay, r2, shootableMask)
                    || !Physics.Raycast(leftWallRay, r2, shootableMask)))
                return characterCrouchHeight;

            return 0;
        }
    }
}