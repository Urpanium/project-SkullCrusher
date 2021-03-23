using System;
using Preferences;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class HumanAI : MonoBehaviour
    {
        private NavMeshAgent navMeshAgent;
        private Transform player;

        private void Awake()
        {
        }

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag(Settings.Tags.Player).transform;
        }

        private void Update()
        {
            // just follow player
            navMeshAgent.destination = player.position;
        }

        private void OnDrawGizmos()
        {
            if(!navMeshAgent)
                return;
            Gizmos.color = Color.blue;
            Vector3 position = transform.position;
            Gizmos.DrawSphere(position, 0.125f);
            for (int i = 0; i < navMeshAgent.path.corners.Length; i++)
            {
                Gizmos.DrawLine(position, navMeshAgent.path.corners[i]);
                position = navMeshAgent.path.corners[i];
            }
        }
    }
}
