using System;
using AI.Enums;
using UnityEngine;

namespace AI.Classes.States.Configs
{
    [Serializable]
    public class IdleStateConfig: AiStateConfig
    {
        public IdleStateType type;

        // if patrolling
        public Transform patrolSetRootTransform;
    }
}