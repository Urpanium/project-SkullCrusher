using UnityEngine;

namespace AI.Scriptables
{
    public class AiBotConfig : ScriptableObject
    {
        [Header("State Settings")] public bool allowIdleState = true;
        public bool allowContactState = true;
        public bool allowAttackState = true;
        public bool allowTakeCoverState = true;
        public bool allowChaseState = true;
        public bool allowSearchState = true;

        public bool allowRearAttackState = true;
        public bool allowHideState = true;
        public bool allowHelpState = true;

        [Range(0, 1)] public float coverPreference = 0.95f;
        [Range(0, 1)] public float grenadePreference = 0.25f;
        [Range(0, 1)] public float weaponChangePreference = 0.25f;
    }
}