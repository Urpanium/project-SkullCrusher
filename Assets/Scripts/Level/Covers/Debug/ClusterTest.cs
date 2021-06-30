using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Level.Covers.Debug
{
    public class ClusterTest : MonoBehaviour
    {
        private CoverManager coverManager;

        private void Start()
        {
            GameObject globalController = GameObject.Find(Preferences.Settings.GameObjects.GlobalController);
            coverManager = globalController.GetComponent<CoverManager>();
        }

        private Vector3 GetPositionOnNavMesh(Vector3 position)
        {
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 20.0f, -1))
                return hit.position;
            return position;
        }


        private void OnDrawGizmosSelected()
        {
            if (!coverManager)
                return;
            Vector3 navMeshPosition =
                GetPositionOnNavMesh(transform.position) + Vector3.up * coverManager.characterCrouchHeight;

            int nearestCluster = coverManager.GetNearestCluster(transform.position);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(navMeshPosition, 0.125f);

            if (nearestCluster != -1)
            {
                Gizmos.color = ColorUtil.RandomColor(nearestCluster);
                Gizmos.DrawCube(coverManager.GetCenter(nearestCluster), Vector3.one);
            }
        }
    }
}