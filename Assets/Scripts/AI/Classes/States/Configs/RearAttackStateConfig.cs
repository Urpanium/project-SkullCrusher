using System;

namespace AI.Classes.States.Configs
{
    [Serializable]
    public class RearAttackStateConfig: AiStateConfig
    {
        public float maximumBypassRadius = 15.0f;
    }
}