using System.Numerics;
using Preferences;
using UnityEngine;
using UnityEngine.AI;
using Weapons;
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


        private void Start()
        {
            Random.InitState(0);

            aiAgent = GetComponent<AiAgent>();
            player = GameObject.FindGameObjectWithTag(Settings.Tags.Player).transform;

            if (idleType == IdleType.Patrol)
                pointSet = GetComponent<AiPatrolPointSet>();

            initialPosition = transform.position;
        }

        private void Update()
        {
            //print(state + " " + aiAgent.GetPlayerVisibility());

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
                state = State.Contact;
            }
        }

        private void AttackingStateUpdate()
        {
            //aiAgent.rotationMode = AiAgent.RotationMode.HeadRules;
            aiAgent.LookAt(player.position);
            lastPlayerPosition = player.position;

            float distance = (player.position - transform.position).magnitude;
            if (distance > aiAgent.visionDistance * 0.5f)
            {
                aiAgent.GoTo(Vector3.Lerp(player.position, transform.position, 0.5f));
            }
            // TODO: shoot this motherfucker
        }

        private void AttackingStateTransitionCheck()
        {
            float playerVisibility = aiAgent.GetPlayerVisibility();
            if (playerVisibility < chaseStartVisibility)
            {
                state = State.Chasing;
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
            Ray ray = new Ray(transform.position + Vector3.up * (aiAgent.agentHeight * 0.5f), horizontalOffset);
            //RaycastHit hit;
            for (int i = 0; i < searchAttemptsCount; i++)
            {
                if (!Physics.Raycast(ray, /*out hit,*/ radius, aiAgent.visibleObjectsMask))
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
    }
}