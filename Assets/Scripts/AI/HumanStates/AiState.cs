using UnityEngine;

namespace AI.States
{
    public abstract class AiState
    {
        public string name;

        public delegate void StateHandler(string stateName);

        public abstract event StateHandler OnStateChange;

        public abstract void Update(Transform agent);
        // some initialization? debug? urr?
        public abstract void OnStateEntered(Transform agent);
    }
}