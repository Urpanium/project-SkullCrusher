using System;
using UnityEngine;

namespace AI.Classes.States.Configs
{
    [Serializable]
    public class SearchStateConfig: AiStateConfig
    {
        [Range(0, 1)] public float detectionPlayerVisibility = 0.2f;
    }
}