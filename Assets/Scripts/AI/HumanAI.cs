using Preferences;
using UnityEngine;
using UnityEngine.AI;
using Weapons;
////////////////////////////////////////////////////
// OLD
////////////////////////////////////////////////////
namespace AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class HumanAI : MonoBehaviour
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
         * Idle -> Contact (when player is far enough or not enough parts of character's model is visible) 
         * Idle -> Attacking (when player is near or enough parts of his model is visible)
         * 
         * Contact -> Attacking (when contactTime is up)
         * 
         * Attacking -> Chasing (when not enough parts of character's model is visible)
         * 
         * Chasing -> Searching (when chaseTime is up)
         * Chasing -> Contact (when player is far enough or not enough parts of character's model is visible)
         * Chasing -> Attacking (when player is near or enough parts of his model is visible) 
         * 
         * Searching -> Attacking (when player is near or enough parts of his model is visible)
         * Searching -> Contact (when player is far enough or not enough parts of character's model is visible)
         * Searching -> Idle (searchTime is up)
         */

        public enum IdleType
        {
            StandAt = 0,
            StandAnywhere,
            PatrolAt,
            RandomWalks,
        }

        public State state;
        public IdleType idleType;
        [Range(0, 1)] public float instantContactVisibility = 0.9f;
        public float instantContactDistance = 5.0f;
        public float contactTime = 3.0f;
        public float chaseTime = 10.0f;
        public float searchTime = 20.0f;

        public Weapon equippedWeapon;

        private float currentContactTime;
        private float currentChaseTime;
        private float currentSearchTime;

        private NavMeshAgent navMeshAgent;
        private Transform player;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag(Settings.Tags.Player).transform;

            currentContactTime = contactTime;
            currentChaseTime = chaseTime;
            currentSearchTime = searchTime;
        }

        private void Update()
        {
            // just follow player
            navMeshAgent.destination = player.position;
            switch (state)
            {
                //TODO: implement all states
                case State.Idle:
                {
                    UpdateIdleState();
                    break;
                }
                case State.Attacking:
                {
                    UpdateAttackingState();
                    break;
                }
                    
            }
        }

        private void UpdateIdleState()
        {
            switch (idleType)
            {
                //TODO: implement all idle types 
                case IdleType.StandAnywhere:
                {
                    // well, just stand
                    break;
                }
                case IdleType.PatrolAt:
                { 
                    break;
                }
                case IdleType.RandomWalks:
                {
                    break;
                }
                case IdleType.StandAt:
                {
                    break;
                }
            }
        }

        private void UpdateAttackingState()
        {
            
        }


        private void LookAt(Vector3 point, float speedMultiplier)
        {
            //TODO: implement this method
        }
        

        private void OnDrawGizmos()
        {
            // draw instant contact sphere
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, instantContactDistance);
            if (!navMeshAgent)
                return;
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