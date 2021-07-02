using AI.Classes.States.Configs;
using UnityEngine;

namespace AI.Classes.States
{
    public class SearchState: AiState
    {

        private SearchStateConfig stateConfig;

        public SearchState(AiStateConfig config, AiBot bot, Transform player): base(config, bot, player)
        {
            name = SearchState;
            stateConfig = (SearchStateConfig) config;
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