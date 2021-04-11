using System;
using AI.HumanStates;
using Preferences;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class Human: MonoBehaviour
    {

        public Transform testPoint;
        public float movementSpeed = 1.0f;
        public float lookSpeed = 1.0f;
        public StateManager stateManager;

        private NavMeshAgent navMeshAgent;
        private Transform player;

        private Vector3 targetMovePosition;
        private Vector3 targetLookPosition;
        

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag(Settings.Tags.Player).transform;
        }

        private void Update()
        {
            
        }

        private void MovementUpdate()
        {
            
        }

        private void LookUpdate()
        {
            
        }

        public void GoTo(Vector3 position)
        {
            navMeshAgent.destination = position;
            targetMovePosition = position;
        }

        public void LookAt(Vector3 position)
        {
            
        }

        private void OnDrawGizmosSelected()
        {
            NavMeshHit hit;
            // TODO: learn about area masks
            if (NavMesh.SamplePosition(testPoint.position, out hit, 10, -1))
            {
                Gizmos.DrawSphere(hit.position, 0.125f);
            }
        }
    }
}