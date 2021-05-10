using System.Collections.Generic;
using Preferences;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AiAgent : MonoBehaviour
    {
        private enum RotationMode
        {
            HeadRules = 0,
            BodyRules,
        }

        // TODO: regroup parameters
        public Transform testPoint;
        public float testRadius = 10.0f;

        public float movementSpeed = 1.0f;
        public float lookSpeed = 1.0f;
        public float bodyRotationSpeed = 2.0f;

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
            LookUpdate();
            if (IsArrivedAtTargetPosition())
            {
                onArrivedAtTargetPosition.Invoke();
            }
        }


        private void LookUpdate()
        {
            // head always rules
            float neckDelta = GetNeckDelta();
            Vector3 targetLookDirection = targetLookPosition - headTransform.position;
            //Vector3 targetLookDirection = headTransform.forward;
            if (neckDelta > maxNeckAngleDelta)
            {
                // neck is about to break
                // need to rotate body
                Vector3 horizontalTargetLookDirection = targetLookDirection;
                horizontalTargetLookDirection.y = 0;
                bodyTransform.rotation = Quaternion.Lerp(bodyTransform.rotation,
                    Quaternion.LookRotation(horizontalTargetLookDirection), bodyRotationSpeed * Time.deltaTime);
            }

            headTransform.forward =
                Vector3.Lerp(headTransform.forward, targetLookDirection, lookSpeed * Time.deltaTime);
        }

        private float GetNeckDelta()
        {
            Vector3 horizontalHeadForward = headTransform.forward;
            horizontalHeadForward.y = 0;
            Vector3 horizontalBodyForward = bodyTransform.forward;
            horizontalBodyForward.y = 0;

            return Vector3.Angle(horizontalHeadForward, horizontalBodyForward);
        }


        public bool IsArrivedAtTargetPosition()
        {
            return (transform.position - targetMovePosition).magnitude < agentHeight;
        }

        public bool IsLookedAtTarget()
        {
            return (transform.forward - (targetLookPosition - transform.position)).magnitude < 0.01f;
        }

        public void StopMoving()
        {
            //targetMovePosition = transform.position;
            GoTo(transform.position);
        }


        public Vector3 GetMovingDirection()
        {
            return navMeshAgent.velocity.normalized;
        }

        // returns possibility to move right into point
        public bool GoTo(Vector3 position)
        {
            //rotationMode = RotationMode.BodyRules;
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
            //rotationMode = RotationMode.HeadRules;
            targetLookPosition = position;
        }

        public void LookAtDirection(Vector3 direction)
        {
            //rotationMode = RotationMode.HeadRules;
            targetLookPosition = headTransform.position + direction;
        }

        public float GetPlayerVisibility()
        {
            float result = 0.0f;
            for (int i = 0; i < playerParts.Count; i++)
            {
                Vector3[] castPoints = GetBoundsPoints(playerParts[i].bounds);
                for (int j = 0; j < castPoints.Length; j++)
                {
                    float visibility = GetVisibilityOfPoint(headTransform, castPoints[j]);
                    result += visibility * perPointMultiplier;
                }
            }

            Vector3 playerDelta = playerTransform.position - transform.position;
            float distanceMultiplier = (visionDistance - playerDelta.magnitude) / visionDistance;

            return result * distanceMultiplier;
        }

        // how good player can see agent
        public float GetInversePlayerVisibility()
        {
            //TODO: maybe make it more complex
            return GetVisibilityOfPoint(playerTransform.GetChild(0), transform.position);
        }

        private float GetVisibilityOfPoint(Transform observerHead, Vector3 point)
        {
            Vector3 pointDelta = (point - observerHead.position);
            float angle = Vector3.Angle(pointDelta.normalized, observerHead.forward);
            //float vision = angle <= visionAngle ? 1 : 0;

            if (angle > visionAngle) return -1;

            Ray ray = new Ray(observerHead.position, pointDelta);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionDistance, visibleObjectsMask))
            {
                float hitDistance = (hit.point - observerHead.position).magnitude;
                float pointDistance = pointDelta.magnitude;
                if (hitDistance > pointDistance)
                    return 1;
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
            if (!navMeshAgent)
                return;


            for (int i = 0; i < playerParts.Count; i++)
            {
                Vector3[] points = GetBoundsPoints(playerParts[i].bounds);
                for (int j = 0; j < points.Length; j++)
                {
                    float visibility = GetVisibilityOfPoint(headTransform, points[j]);
                    float redComponent = Mathf.Clamp01(visibility);
                    float greenComponent = 0;
                    if (visibility < 0)
                        greenComponent = 1;

                    Gizmos.color = new Color(redComponent, greenComponent, 0.0f);
                    Gizmos.DrawSphere(points[j], 0.125f);

                    //Gizmos.DrawLine(points[j], headTransform.position);
                }
            }

            Gizmos.color = Color.blue;
            Vector3 position = transform.position;
            Gizmos.DrawSphere(position, 0.125f);
            foreach (var t in navMeshAgent.path.corners)
            {
                Gizmos.DrawLine(position, t);
                position = t;
            }
        }
    }
}