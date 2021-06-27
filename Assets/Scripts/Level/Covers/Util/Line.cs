using System;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level.Covers.Util
{
    [Serializable]
    public class Line
    {
        public Vector3 point1;
        public Vector3 point2;

        public float Length => Vector3.Distance(point1, point2);

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

        public bool LineInside(Line other, float epsilon = 0.01f)
        {
            float point1 = PointOnLine(this, other.point1);
            float point2 = PointOnLine(this, other.point2);
            if (point1 > 0 || point2 > 0)
                return false;

            return Length > other.Length && point1 * point2 > 1 - epsilon;
        }

        public bool LineSemiInside(Line other, float epsilon = 0.01f)
        {
            Vector3 direction1 = (point1 - point2).normalized;
            Vector3 direction2 = (other.point1 - other.point2).normalized;
            
            float dot = Vector3.Dot(direction1, direction2);
            
            if (dot * dot < 1 - epsilon)
            {
                return false;
            }

            float point1f = PointOnLine(this, other.point1);
            float point2f = PointOnLine(this, other.point2);
            /*if (point1 > 0 || point2 > 0)
                return false;*/

            return point1f * -1 > 1 - epsilon || point2f * -1 > 1 - epsilon;
        }

        private static float PointOnLine(Line line, Vector3 testPoint)
        {
            Vector3 direction1 = (testPoint - line.point1).normalized;
            Vector3 direction2 = (testPoint - line.point2).normalized;

            float dot = Vector3.Dot(direction1, direction2);

            return dot;
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