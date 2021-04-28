using System;
using UnityEngine;

namespace AI
{
    public class AiPatrolPointSet : MonoBehaviour
    {
        public Transform pointsParentTransform;
        public Vector3[] points;
        public bool updateEveryFrame = false;

        private void Start()
        {
            GetPoints();
        }

        private void Update()
        {
            if(updateEveryFrame)
                GetPoints();
        }


        private void GetPoints()
        {
            if (!pointsParentTransform)
                return;
            if (points == null)
                points = new Vector3[pointsParentTransform.childCount];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = pointsParentTransform.GetChild(i).position;
            }
        }
    }
}