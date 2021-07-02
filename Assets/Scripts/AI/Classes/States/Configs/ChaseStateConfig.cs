using System;
using UnityEngine;

namespace AI.Classes.States.Configs
{
    [Serializable]
    public class ChaseStateConfig: AiStateConfig
    {
        [Range(0, 1)] public float contactLosePlayerVisibility = 0.15f;
        public float time = 10.0f;
    }
}