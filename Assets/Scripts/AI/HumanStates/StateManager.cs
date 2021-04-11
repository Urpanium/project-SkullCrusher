using System;
using AI.States;
using UnityEngine;

namespace AI.HumanStates
{
    [Serializable]
    public class StateManager
    {
        [Range(0, 1)] public float instantContactVisibility = 0.9f;
        public float instantContactDistance = 5.0f;
        public float contactTime = 3.0f;
        public float chaseTime = 10.0f;
        public float searchTime = 20.0f;
        //TODO: just list all types of states to assign parameters
        public AiState[] states;
        
    }
}