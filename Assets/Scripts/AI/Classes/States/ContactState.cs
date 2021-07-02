using AI.Classes.Groups.GroupRoles;
using AI.Classes.States.Configs;
using AI.Enums;
using UnityEngine;

namespace AI.Classes.States
{
    public class ContactState: AiState
    {

        private ContactStateConfig stateConfig;

        private float contactTime;
        
        public ContactState(AiStateConfig config, AiBot bot, Transform player): base(config, bot, player)
        {
            name = ContactState;
            stateConfig = (ContactStateConfig) config;
        }
        
        public override void Update()
        {
            bot.controller.LookAt(player.position);
            contactTime += Time.deltaTime * bot.controller.GetPlayerVisibilityDistanceMultiplier();
        }

        public override string TransitionCheck()
        {
            // do not forget to reset contactTime
            float playerVisibility = bot.controller.GetPlayerVisibility();
            if (playerVisibility > stateConfig.instantContactPlayerVisibility || contactTime > stateConfig.contactTime)
            {
                contactTime = 0.0f; 
                /* TODO: group behaviour
                if (bot.IsInGroup())
                {
                    AiBotGroupRole role = bot.groupRole;
                    
                }
                */
                return AttackState;
            }
            return KeepCurrentState;
        }
    }
}