using System;
using Level.Covers.Util;
using UnityEngine;

namespace Level.Covers
{
    public class LineTest : MonoBehaviour
    {
        public Transform point1;
        public Transform point2;

        public Transform point3;
        public Transform point4;

        public float lineMergeEpsilon = 0.01f;


        public bool inside;
        public bool semiInside;
        public float dot;

        private Line line1;
        private Line line2;

        private void Start()
        {
            line1 = new Line(point1.position, point2.position);
            line2 = new Line(point3.position, point4.position);
        }

        private void Update()
        {
            line1.point1 = point1.position;
            line1.point2 = point2.position;
            line2.point1 = point3.position;
            line2.point2 = point4.position;

            inside = false;
            semiInside = false;

            Vector3 d1 = (line1.point1 - line1.point2).normalized;
            Vector3 d2 = (line2.point1 - line2.point2).normalized;

            dot = Vector3.Dot(d1, d2);

            if (line1.LineInside(line2, lineMergeEpsilon))
            {
                inside = true;
                return;
            }

            if (line2.LineInside(line1, lineMergeEpsilon))
            {
                inside = true;
                return;
            }

            if (line1.LineSemiInside(line2, lineMergeEpsilon) || line2.LineSemiInside(line1, lineMergeEpsilon))
            {
                Vector3[] pairs =
                {
                    line1.point1, line2.point1,
                    line1.point1, line2.point2,

                    line1.point2, line2.point1,
                    line1.point2, line2.point2
                };

                int longestPairIndex = 0;
                for (int i = 2; i < pairs.Length; i += 2)
                {
                    Vector3 v1 = pairs[i];
                    Vector3 v2 = pairs[i + 1];

                    Vector3 cv1 = pairs[longestPairIndex];
                    Vector3 cv2 = pairs[longestPairIndex + 1];
                    if (Vector3.Distance(v1, v2) > Vector3.Distance(cv1, cv2))
                    {
                        longestPairIndex = i;
                    }
                }

                Vector3 l1 = pairs[longestPairIndex];
                Vector3 l2 = pairs[longestPairIndex + 1];   
                UnityEngine.Debug.DrawLine(l1, l2);
                semiInside = true;
            }
        }


        private void OnDrawGizmos()
        {
            if (inside)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(transform.position, 0.125f);
            }

            if (semiInside)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position + Vector3.up, 0.125f);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(line1.point1, line1.point2);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(line2.point1, line2.point2);
        }
    }
}