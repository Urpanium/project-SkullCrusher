using System;
using AI.HumanStates;
using Preferences;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AI
{
    public class Agent: MonoBehaviour
    {

        public Transform testPoint;
        public float testRadius = 10.0f; 
        public float movementSpeed = 1.0f;
        public float lookSpeed = 1.0f;
        public StateManager stateManager;

        public float agentHeight = 2.0f;
        public float sampleRadius = 15.0f;


        public float maxNeckAngleDelta = 72.0f; // google didn't help, there is no information about human's max neck rotation angle 
        public Transform headTransform;
        public Transform bodyTransform;

        public UnityEvent onArrivedAtTargetPosition;

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
            LookUpdate();
            
            if(IsArrivedAtTargetPosition())
                onArrivedAtTargetPosition.Invoke();
            
        }
        

        private void LookUpdate()
        {
            Vector3 horizontalHeadForward = headTransform.forward;
            horizontalHeadForward.y = 0;
            Vector3 horizontalBodyForward = bodyTransform.forward;
            horizontalBodyForward.y = 0;
            
            float neckDelta = Vector3.Angle(horizontalHeadForward, horizontalBodyForward);
            Vector3 targetLookDirection = targetLookPosition - headTransform.position;
            if (neckDelta > maxNeckAngleDelta)
            {
                // neck is about to break
                // need to rotate body
                Vector3 horizontalTargetLookDirection = targetLookDirection;
                horizontalTargetLookDirection.y = 0;
                bodyTransform.rotation = Quaternion.Lerp(bodyTransform.rotation, Quaternion.LookRotation(horizontalTargetLookDirection), lookSpeed * Time.deltaTime);
            }

            headTransform.forward = Vector3.Lerp(headTransform.forward, targetLookDirection, lookSpeed * Time.deltaTime);

        }

        private bool IsArrivedAtTargetPosition()
        {
            return (transform.position - targetMovePosition).magnitude < agentHeight;
        }

        // returns possibility to move right into point
        public bool GoTo(Vector3 position)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, sampleRadius, -1))
            {
                if ((hit.position - position).magnitude > agentHeight)
                {

                    navMeshAgent.destination = hit.position;
                    targetMovePosition = hit.position;
                    return false;
                }
            }

            navMeshAgent.destination = position;
            targetMovePosition = position;
            
            return true;
        }

        public void LookAt(Vector3 position)
        {
            targetLookPosition = position;
        }

        public void LookAtDirection(Vector3 direction)
        {
            // remove if useless
            throw new NotImplementedException();
        }

        private void OnDrawGizmosSelected()
        {
            NavMeshHit hit;
            // TODO: learn about area masks
            Gizmos.DrawWireSphere(testPoint.position, testRadius);
            if (NavMesh.SamplePosition(testPoint.position, out hit, testRadius, -1))
            {
                Gizmos.DrawSphere(hit.position, 0.125f);
            }
        }
    }
}