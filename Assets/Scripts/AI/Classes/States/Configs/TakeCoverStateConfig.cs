using System;
using UnityEngine;

namespace AI.Classes.States.Configs
{
    [Serializable]
    public class TakeCoverStateConfig : AiStateConfig
    {
        //public bool botCanCrouch;
        [Range(0, 90)] public float angleThreshold = 15.0f;
    }
}