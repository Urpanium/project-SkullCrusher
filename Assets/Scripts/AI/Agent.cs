using System;
using System.Collections.Generic;
using AI.HumanStates;
using Preferences;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace AI
{
    public class Agent : MonoBehaviour
    {
        // TODO: regroup parameters
        public Transform testPoint;
        public float testRadius = 10.0f;
        public float movementSpeed = 1.0f;
        public float lookSpeed = 1.0f;
        public StateManager stateManager;


        public float agentHeight = 2.0f;
        public float sampleRadius = 15.0f;

        /* visionAngle
         * \ 100 /
         *  \  /
         *   o
         */
        public float visionAngle = 50.0f;

        public float visionDistance = 50.0f;

        //do not include player
        public LayerMask visibleObjectsMask;


        public float
            maxNeckAngleDelta =
                72.0f; // google didn't help, there is no information about human's max neck rotation angle 

        public Transform headTransform;
        public Transform bodyTransform;

        public UnityEvent onArrivedAtTargetPosition;

        private NavMeshAgent navMeshAgent;
        private Transform playerTransform;

        private List<MeshRenderer> playerParts;
        private float perPointMultiplier;

        private Vector3 targetMovePosition;
        private Vector3 targetLookPosition;


        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            playerTransform = GameObject.FindGameObjectWithTag(Settings.Tags.Player).transform;

            playerParts = new List<MeshRenderer>();
            GetPlayerParts(playerTransform);
            perPointMultiplier = 1.0f / (playerParts.Count * 8);
        }

        private void Update()
        {
            //LookUpdate();
            print(GetPlayerVisibility());
            if (IsArrivedAtTargetPosition())
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
                bodyTransform.rotation = Quaternion.Lerp(bodyTransform.rotation,
                    Quaternion.LookRotation(horizontalTargetLookDirection), lookSpeed * Time.deltaTime);
            }

            headTransform.forward =
                Vector3.Lerp(headTransform.forward, targetLookDirection, lookSpeed * Time.deltaTime);
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

        private float GetPlayerVisibility()
        {
            float result = 0.0f;
            for (int i = 0; i < playerParts.Count; i++)
            {
                Vector3[] castPoints = GetBoundsPoints(playerParts[i].bounds);
                for (int j = 0; j < castPoints.Length; j++)
                {
                    float visibility = GetVisibilityOfPoint(castPoints[j]);
                    result += visibility * perPointMultiplier;
                }
            }


            return result;
        }

        private float GetVisibilityOfPoint(Vector3 point)
        {
            Vector3 pointDelta = point - headTransform.position;
            float angle = Vector3.Angle(pointDelta.normalized, headTransform.forward);
            float vision = angle < visionAngle ? 1 : 0;

            if (!(vision > 0)) return 0;

            Ray ray = new Ray(headTransform.position, pointDelta);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionDistance, visibleObjectsMask))
            {
                float hitDistance = (headTransform.position - hit.point).sqrMagnitude;
                float pointDistance = pointDelta.sqrMagnitude;
                if (hitDistance > pointDistance)
                    return vision;
            }

            return 0;
        }

        private Vector3[] GetBoundsPoints(Bounds bounds)
        {
            return new[]
            {
                bounds.max,
                bounds.min,

                new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
                new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),

                new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
                new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.min.z)
            };
        }

        private void GetPlayerParts(Transform currentPlayerTransform)
        {
            MeshRenderer meshRenderer = currentPlayerTransform.GetComponent<MeshRenderer>();
            if (meshRenderer)
                playerParts.Add(meshRenderer);

            for (int i = 0; i < currentPlayerTransform.childCount; i++)
            {
                GetPlayerParts(currentPlayerTransform.GetChild(i));
            }
        }


        private void OnDrawGizmos()
        {
            if (playerParts == null)
                return;
            NavMeshHit hit;
            // TODO: learn about area masks
            Gizmos.DrawWireSphere(testPoint.position, testRadius);
            if (NavMesh.SamplePosition(testPoint.position, out hit, testRadius, -1))
            {
                Gizmos.DrawSphere(hit.position, 0.125f);
            }


            for (int i = 0; i < playerParts.Count; i++)
            {
                Vector3[] points = GetBoundsPoints(playerParts[i].bounds);
                for (int j = 0; j < points.Length; j++)
                {
                    float visibility = GetVisibilityOfPoint(points[j]);
                    float redComponent = visibility;

                    Gizmos.color = new Color(redComponent, 0.0f, 0.0f);
                    Gizmos.DrawSphere(points[j], 0.125f);
                }
            }

            //TODO: draw visibility area

            //TODO: make script that will draw player visibility boxes
        }
    }
}