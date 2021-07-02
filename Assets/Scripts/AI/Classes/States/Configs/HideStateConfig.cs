using System;
using UnityEngine;

namespace AI.Classes.States.Configs
{
    [Serializable]
    public class HideStateConfig: AiStateConfig
    {
        [Range(0, 1)] public float healthThreshold = 0.1f;
    }
}