using System;
using AI.States;
using UnityEngine;

namespace AI.HumanStates
{
    [Serializable]
    public class IdleState: AiState
    {

        public enum IdleType
        {
            Stay = 0,
            StayAt,
            Patrol,
            RandomWalk
        }

        public IdleType idleType;
        // must be empty if not patrol idle type was selected
        public Transform patrolPoints;

        public override event StateHandler OnStateChange;

        public override void Update(Transform agent)
        {
            
            throw new NotImplementedException();
        }

        private void UpdateStay(Transform agent)
        {
            // well, do nothing
        }

        public override void OnStateEntered(Transform agent)
        {
            throw new NotImplementedException();
        }
    }
}