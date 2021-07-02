using System;
using UnityEngine;

namespace AI.Classes.States.Configs
{
    [Serializable]
    public class ContactStateConfig : AiStateConfig
    {
        [Header("Player Spot Settings")] public LayerMask visibleObjectsMask; 
        [Range(0, 1)] public float instantContactPlayerVisibility = 0.95f;
        [Range(0, 1)] public float contactStartPlayerVisibility = 0.25f;
        public float visionDistance = 50.0f;
        [Range(0, 90)] public float visionAngle = 60.0f;
        public float contactTime = 2.0f;
    }
}