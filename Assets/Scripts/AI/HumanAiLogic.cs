using System.Collections.Generic;
using Character;
using Preferences;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using Weapons;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

////////////////////////////////////////////////////
// OLD
////////////////////////////////////////////////////
namespace AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AiAgent))]
    public class HumanAiLogic : MonoBehaviour
    {
        public enum State
        {
            Idle = 0, // patrols or stands still
            Contact, // player is visible, waiting for spot time to end up (if player is far enough), maybe playing some noises to notify player
            Attacking, // player is spotted, bot's position is convenient to shoot, if not, try to walk to player or distance from him
            // RetreatingAttacking, // attacking state, but player it too close, trying to avoid him
            TakingCover, // fires out what remains in weapon's clip while going to nearest cover
            Chasing, // player is not visible, trying to establish contact before time is up, try to walk to player's last position 
            Searching // player is lost, try to search him in nearest positions for a while
        }

        /*
         * (DONE) Idle -> Contact (when player is far enough or not enough parts of character's model is visible) 
         * (DONE) Idle -> Attacking (when player is near or enough parts of his model is visible)
         * 
         * (DONE) Contact -> Attacking (when contactTime is up)
         * Contact -> Idle
         * 
         * (DONE) Attacking -> Chasing (when not enough parts of character's model is visible)
         * Attacking -> TakingCover (when less than X percent of ammo remained in current clip)
         * (REMOVED) Attacking -> RetreatAttacking (if distance < retreatDistance)
         *
         * TakingCover -> Attacking (when reload is done)
         * 
         * (DONE) Chasing -> Searching (when chaseTime is up)
         * (REMOVED) Chasing -> Contact (when player is far enough or not enough parts of character's model is visible)
         * (DONE) Chasing -> Attacking (when player is near or enough parts of his model is visible) 
         * 
         * Searching -> Attacking (when player is near or enough parts of his model is visible)
         * (REMOVED) Searching -> Contact (when player is far enough or not enough parts of character's model is visible)
         * Searching -> Idle (searchTime is up)
         */

        public enum IdleType
        {
            Stand = 0,
            Patrol,
        }

        public State state;
        public IdleType idleType;

        [Range(0, 1)] public float instantContactVisibility = 0.9f;
        public float contactStartVisibility = 0.2f;
        public float chaseStartVisibility = 0.1f;

        public float contactTime = 3.0f;
        public float chaseTime = 10.0f;
        public float searchTime = 20.0f;
        public float searchPointChangeInterval = 15.0f;
        [Range(1, 15)] public int searchAttemptsCount = 3;
        public float searchRadius = 20.0f;

        public float coverSearchDistance = 10.0f;
        [Range(1, 15)] public int coverSearchSamples = 5;
        [Range(0.01f, 1.99f)] public float coverMinThickness = 0.25f;
        [Range(0.02f, 2.0f)] public float coverMaxThickness = 2.0f;

        [Tooltip("How close player must be to start retreating. Set to negative for weapon standard.")]
        // TODO: implement negative variant
        public float retreatDistance = 5.0f;

        [Range(1, 15)] public int retreatSearchSamples = 10;
        public LayerMask coverLayerMask;

        public Transform armPivot;
        public Weapon equippedWeapon;


        // TODO: coroutines?
        private float currentContactTime;
        private float currentChaseTime;

        private float currentSearchIntervalTime;
        private float currentSearchTime;

        private Vector3 initialPosition;

        // chasing player when it's gone
        private Vector3 lastPlayerPosition;

        // if idle type is patrol
        private AiPatrolPointSet pointSet;

        //private NavMeshAgent navMeshAgent;
        private AiAgent aiAgent;
        private Transform player;
        private Controller controller;

        private void Start()
        {
            Random.InitState(0);

            aiAgent = GetComponent<AiAgent>();
            
            player = GameObject.FindGameObjectWithTag(Settings.Tags.Player).transform;
            controller = player.GetComponent<Controller>();
            if (idleType == IdleType.Patrol)
                pointSet = GetComponent<AiPatrolPointSet>();

            initialPosition = transform.position;
            EquipWeapon(equippedWeapon);
        }

        public void EquipWeapon(Weapon weapon)
        {
            DropEquippedWeapon();
            Transform weaponTransform = weapon.transform;
            equippedWeapon = weapon;
            equippedWeapon.isEquipped = true;
            weaponTransform.parent = armPivot;
            weaponTransform.localPosition = weapon.offset;
            weaponTransform.forward = armPivot.forward;
            weaponTransform.GetComponent<Rigidbody>().isKinematic = true;
        }

        public void DropEquippedWeapon()
        {
            // TODO: implement
        }

        private void Update()
        {

            switch (state)
            {
                //TODO: implement all states
                case State.Idle:
                {
                    IdleStateUpdate();
                    IdleStateTransitionCheck();
                    break;
                }
                case State.Attacking:
                {
                    AttackingStateUpdate();
                    AttackingStateTransitionCheck();
                    break;
                }
                case State.TakingCover:
                {
                    TakingCoverStateUpdate();
                    TakingCoverStateTransitionCheck();
                    break;
                }
                case State.Chasing:
                {
                    ChasingStateUpdate();
                    ChasingStateTransitionCheck();
                    break;
                }
                case State.Contact:
                {
                    ContactStateUpdate();
                    ContactStateTransitionCheck();
                    break;
                }
                case State.Searching:
                {
                    SearchingStateUpdate();
                    SearchingStateTransitionCheck();
                    break;
                }
            }
        }


        private void IdleStateUpdate()
        {
            switch (idleType)
            {
                case IdleType.Stand:
                {
                    // return to initial position if possible
                    aiAgent.GoTo(initialPosition);
                    //aiAgent.rotationMode = AiAgent.RotationMode.BodyRules;
                    break;
                }
                case IdleType.Patrol:
                {
                    // return to patrol positions
                    break;
                }
            }
        }

        private void IdleStateTransitionCheck()
        {
            // TODO: check conditions to change state

            float playerVisibility = aiAgent.GetPlayerVisibility();

            if (playerVisibility > instantContactVisibility)
            {
                state = State.Attacking;
            }


            if (playerVisibility > contactStartVisibility)
            {
                aiAgent.StopMoving();
                state = State.Contact;
            }
        }

        private void AttackingStateUpdate()
        {
            //aiAgent.rotationMode = AiAgent.RotationMode.HeadRules;
            aiAgent.LookAt(player.position);
            lastPlayerPosition = player.position + controller.GetVelocity().normalized;

            float distance = (player.position - transform.position).magnitude;
            if (distance < retreatDistance)
            {
                Vector3 retreatPoint;
                if (TryGetRetreatPoint(out retreatPoint))
                {
                    aiAgent.GoTo(retreatPoint);
                }
            }
            if (distance > aiAgent.visionDistance * 0.5f)
            {
                aiAgent.GoTo(Vector3.Lerp(player.position, transform.position, 0.5f));
            }

            // TODO: shoot this motherfucker

            equippedWeapon.StandardFire();
        }

        private void AttackingStateTransitionCheck()
        {
            float playerVisibility = aiAgent.GetPlayerVisibility();
            if (playerVisibility < chaseStartVisibility)
            {
                state = State.Chasing;
            }
            
            if (equippedWeapon.currentClipAmmoAmount == 0)
            {
                if (equippedWeapon.currentRemainedAmmoAmount < equippedWeapon.weaponParameters.clipAmmoAmount)
                {
                    equippedWeapon.currentRemainedAmmoAmount += equippedWeapon.weaponParameters.clipAmmoAmount;
                }

                state = State.TakingCover;
            }
        }

        private void TakingCoverStateUpdate()
        {
            Vector3 pointToGo;
            if (!IsInCoverPoint())
            {
                if (TryGetCoverPoint(out pointToGo))
                {
                    aiAgent.GoTo(pointToGo);
                    return;
                }

                print("Can't find cover point");

                if (aiAgent.IsArrivedAtTargetPosition() && TryGetRandomPointToSearch(out pointToGo))
                {
                    aiAgent.GoTo(pointToGo);
                    return;
                }
            }

            // well, guess i'll die
        }

        private void TakingCoverStateTransitionCheck()
        {
            if (equippedWeapon.currentClipAmmoAmount > 0)
            {
                state = State.Attacking;
            }
        }


        private void ChasingStateUpdate()
        {
            //aiAgent.rotationMode = AiAgent.RotationMode.HeadRules;
            if (aiAgent.GetPlayerVisibility() > contactStartVisibility)
            {
                lastPlayerPosition = player.position;
            }

            aiAgent.GoTo(lastPlayerPosition);

            currentChaseTime += Time.deltaTime;
        }

        private void ChasingStateTransitionCheck()
        {
            if (currentChaseTime > chaseTime)
            {
                currentChaseTime = 0.0f;
                state = State.Searching;
            }

            if (aiAgent.IsArrivedAtTargetPosition())
            {
                currentChaseTime = 0.0f;
                state = State.Searching;
            }

            // we know that player is here so we paying more attention
            if (aiAgent.GetPlayerVisibility() > contactStartVisibility)
            {
                aiAgent.StopMoving();
                currentChaseTime = 0.0f;
                state = State.Attacking;
            }
        }

        private void ContactStateUpdate()
        {
            aiAgent.LookAt(player.position);
            currentContactTime += Time.deltaTime;
        }

        private void ContactStateTransitionCheck()
        {
            float playerVisibility = aiAgent.GetPlayerVisibility();
            if (currentContactTime > contactTime || playerVisibility > instantContactVisibility)
            {
                currentContactTime = 0.0f;
                state = State.Attacking;
            }

            if (playerVisibility < contactStartVisibility)
            {
                currentContactTime = 0.0f;
                state = State.Idle;
            }
        }

        private void SearchingStateUpdate()
        {
            currentSearchTime += Time.deltaTime;
            currentSearchIntervalTime += Time.deltaTime;
            if (aiAgent.IsLookedAtTarget())
            {
                Vector3 lookDirection = Random.insideUnitSphere;
                lookDirection.y *= 2;
                lookDirection.Normalize();
                aiAgent.LookAtDirection(lookDirection);
            }

            if (currentSearchIntervalTime > searchPointChangeInterval && aiAgent.IsArrivedAtTargetPosition())
            {
                Vector3 pointToSearch;
                bool successful = TryGetRandomPointToSearch(out pointToSearch);
                if (successful)
                {
                    currentSearchIntervalTime = 0.0f;
                    aiAgent.GoTo(pointToSearch);

                    Vector3 lookDirection = pointToSearch - transform.position;
                    lookDirection.y = 0;
                    aiAgent.LookAtDirection(lookDirection.normalized);
                }
            }
        }

        private void SearchingStateTransitionCheck()
        {
            if (currentSearchTime > searchTime)
            {
                currentSearchTime = 0.0f;
                currentSearchIntervalTime = 0.0f;
                state = State.Idle;
            }

            // also paying more attention
            if (aiAgent.GetPlayerVisibility() > contactStartVisibility)
            {
                currentSearchTime = 0.0f;
                currentSearchIntervalTime = 0.0f;
                aiAgent.StopMoving();
                state = State.Attacking;
            }
        }


        private bool TryGetRandomPointToSearch(out Vector3 point)
        {
            float radius = Random.value * searchRadius;
            Vector3 offset = Random.insideUnitSphere * radius;
            Vector3 horizontalOffset = offset;
            horizontalOffset.y = 0;
            Ray ray1 = new Ray(transform.position + Vector3.up * (aiAgent.agentHeight * 0.5f), horizontalOffset);
            Ray ray2 = new Ray(transform.position - Vector3.up * (aiAgent.agentHeight * 0.499f), horizontalOffset);
            //RaycastHit hit;
            for (int i = 0; i < searchAttemptsCount; i++)
            {
                if (!Physics.Raycast(ray1, /*out hit,*/ radius, aiAgent.visibleObjectsMask)
                    || !Physics.Raycast(ray2, radius, aiAgent.visibleObjectsMask))
                {
                    /*float hitDistance = (hit.point - transform.position).magnitude;
                    if()*/
                    //return transform.position;
                    point = transform.position + offset;
                    return true;
                }
            }

            point = transform.position;
            return false;
        }

        private bool TryGetRetreatPoint(out Vector3 point)
        {
            Vector3 playerDelta = transform.position - player.position;
            playerDelta.y = 0;

            Vector3 playerDeltaRight = Vector3.Cross(playerDelta.normalized, Vector3.up);
            float retreatAtDistance = retreatDistance - playerDelta.magnitude;
            float axisStep = Mathf.Deg2Rad * 180.0f / retreatSearchSamples;
            
            // first of all try playerDelta direction
            Ray ray1 = new Ray(transform.position + Vector3.up * (aiAgent.agentHeight * 0.5f), playerDelta);
            Ray ray2 = new Ray(transform.position - Vector3.up * (aiAgent.agentHeight * 0.499f), playerDelta);

            if (!Physics.Raycast(ray1, retreatAtDistance, aiAgent.visibleObjectsMask)
                || !Physics.Raycast(ray2, retreatAtDistance, aiAgent.visibleObjectsMask))
            {
                point = transform.position + playerDelta.normalized * retreatAtDistance;
                return true;
            }
            playerDelta.Normalize();
            for (int i = 0; i < retreatSearchSamples; i++)
            {
                float t = i * axisStep;

                float x = Mathf.Sin(t);
                float y = Mathf.Cos(t);
                Vector3 direction = playerDelta * y + playerDeltaRight * x;
                ray1 = new Ray(transform.position + Vector3.up * (aiAgent.agentHeight * 0.5f), direction);
                ray2 = new Ray(transform.position - Vector3.up * (aiAgent.agentHeight * 0.499f), direction);
                if (!Physics.Raycast(ray1, retreatAtDistance, aiAgent.visibleObjectsMask)
                    || !Physics.Raycast(ray2, retreatAtDistance, aiAgent.visibleObjectsMask))
                {
                    point = transform.position + direction * retreatAtDistance;
                    return true;
                }
            }

            point = Vector3.zero;
            return false;
        }


        private bool IsInCoverPoint()
        {
            //TODO: make it more complex
            float inversePlayerVisibility = aiAgent.GetInversePlayerVisibility();
            return inversePlayerVisibility < 0.25f;
        }

        //TODO: consider moving to AiAgent class
        private bool TryGetCoverPoint(out Vector3 point)
        {
            float axisStep = coverSearchDistance / coverSearchSamples;


            List<Vector3> variants = new List<Vector3>();
            // TODO: use positions delta instead of player forward
            Vector3 playerDelta = transform.position - player.position;
            playerDelta.y = 0;

            Vector3 playerDeltaRight = Vector3.Cross(playerDelta.normalized, Vector3.up);
            float initialOffset = -0.5f * coverSearchDistance;
            for (int i = 0; i < coverSearchSamples; i++)
            {
                float currentOffset = i * axisStep;
                Vector3 direction = (playerDelta - playerDeltaRight * (initialOffset + currentOffset)).normalized;
                Transform playerHeadPivot = player.GetChild(0);
                Ray ray = new Ray(playerHeadPivot.position, direction);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, coverSearchDistance, coverLayerMask))
                {
                    // we (NEED TO) hit a wall
                    Vector3 possibleCoverPosition = hit.point + direction.normalized * coverMaxThickness;
                    Ray inversedRay = new Ray(possibleCoverPosition, -direction);
                    RaycastHit inversedHit;
                    if (Physics.Raycast(inversedRay, out inversedHit, coverSearchDistance, coverLayerMask))
                    {
                        Vector3 delta = hit.point - inversedHit.point;
                        float thickness = delta.magnitude;
                        // TODO: add max thickness??
                        if (thickness > coverMinThickness)
                        {
                            // thicc
                            variants.Add(possibleCoverPosition);
                        }
                    }
                }
            }

            if (variants.Count == 0)
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 bestCoverPoint = variants[0];
            float bestCoverPointDistance = coverSearchDistance;
            foreach (Vector3 coverPoint in variants)
            {
                Vector3 delta = coverPoint - transform.position;
                float distance = delta.sqrMagnitude;
                if (distance < bestCoverPointDistance)
                {
                    bestCoverPoint = coverPoint;
                    bestCoverPointDistance = distance;
                }
            }

            point = bestCoverPoint;
            return true;
        }


        private void OnDrawGizmos()
        {
            if(!player)
                return;
            
            Vector3 playerDelta = transform.position - player.position;
            playerDelta.y = 0;
            // just swap x and z
            Vector3 playerDeltaRight = Vector3.Cross(playerDelta.normalized, Vector3.up);
            Gizmos.DrawLine(transform.position, transform.position + playerDelta.normalized);
            Gizmos.DrawLine(transform.position, transform.position + playerDeltaRight.normalized);
        }
    }
}