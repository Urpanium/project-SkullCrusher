using System;
using UnityEngine;

namespace AI.Classes.States.Configs
{
    [Serializable]
    public class ChaseStateConfig: AiStateConfig
    {
        [Range(0, 1)] public float detectionPlayerVisibility = 0.15f;
    }
}