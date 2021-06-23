﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level.Covers.Util
{
    [Serializable]
    public class Line
    {
        public Vector3 point1;
        public Vector3 point2;

        public Line(Vector3 point1, Vector3 point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }


        public bool HasPoint(Vector3 point, float epsilon = 0.01f)
        {
            return Vector3.Distance(point1, point) < epsilon ||
                   Vector3.Distance(point2, point) < epsilon;
        }

        public void ChangePoint(Vector3 original, Vector3 newValue, float epsilon = 0.01f)
        {
            if (Vector3.Distance(point1, original) < epsilon)
                point1 = newValue;
            if (Vector3.Distance(point2, original) < epsilon)
                point2 = newValue;
        }

        public bool HasSamePoints(Line other, float epsilon = 0.01f)
        {
            return Vector3.Distance(point1, other.point1) < epsilon || Vector3.Distance(point2, other.point2) < epsilon
                                                                    || Vector3.Distance(point1, other.point2) <
                                                                    epsilon || Vector3.Distance(point2, other.point1) <
                                                                    epsilon;
        }

        public (Vector3, Vector3, Vector3) GetSameAndDifferentPoints(Line other, float epsilon = 0.01f)
        {
            // (same, different, different)

            if (Vector3.Distance(point1, other.point1) < epsilon)
                return (point1, point2, other.point2);
            if (Vector3.Distance(point2, other.point2) < epsilon)
                return (point2, point1, other.point1);
            if (Vector3.Distance(point1, other.point2) < epsilon)
                return (point1, point2, other.point1);
            return (point2, point1, other.point2);
        }


        public bool Equals(Line other, float epsilon = 0.01f)
        {
            return Vector3.Distance(point1, other.point1) < epsilon && Vector3.Distance(point2, other.point2) < epsilon
                   || Vector3.Distance(point1, other.point2) < epsilon &&
                   Vector3.Distance(point2, other.point1) < epsilon;
        }
    }
}