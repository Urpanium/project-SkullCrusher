using AI.Classes.States.Configs;
using UnityEngine;

namespace AI.Classes.States
{
    public class ChaseState: AiState
    {

        private ChaseStateConfig stateConfig;

        public ChaseState(AiStateConfig config, AiBot bot, Transform player): base(config, bot, player)
        {
            name = ChaseState;
            stateConfig = (ChaseStateConfig) config;
        }
        
        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override string TransitionCheck()
        {
            throw new System.NotImplementedException();
        }
    }
}