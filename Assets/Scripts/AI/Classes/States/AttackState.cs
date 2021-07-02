using AI.Classes.States.Configs;
using UnityEngine;

namespace AI.Classes.States
{
    public class AttackState : AiState
    {
        private AttackStateConfig stateConfig;

        private ChaseStateConfig chaseStateConfig;

        public AttackState(AiStateConfig config, AiBot bot, Transform player) : base(config, bot, player)
        {
            name = AttackState;
            stateConfig = (AttackStateConfig) config;
            chaseStateConfig = bot.config.chaseStateConfig;
        }

        public override void Update()
        {
            Vector3 playerPosition = player.position;
            bot.controller.LookAt(playerPosition);
            float distance = (playerPosition - bot.transform.position).sqrMagnitude;
            if (distance < stateConfig.retreatDistance * stateConfig.retreatDistance)
            {
                if (TryGetRetreatPoint(out Vector3 retreatPoint))
                {
                    bot.controller.GoTo(retreatPoint);
                }
            }

            if (distance > bot.controller.visionDistance * stateConfig.playerFollowVisionDistancePart)
            {
                bot.controller.GoTo(Vector3.Lerp(bot.transform.position, player.position,
                    stateConfig.playerFollowVisionDistancePart));
            }

            bot.Fire();
        }

        public override string TransitionCheck()
        {
            float playerVisibility = bot.controller.GetPlayerVisibility();
            if (playerVisibility > chaseStateConfig.contactLosePlayerVisibility)
            {
                return ChaseState;
            }

            if (bot.IsNeedToReload())
            {
                return TakeCoverState;
            }
            
            // TODO: transition to HideState

            return KeepCurrentState;
        }


        private bool TryGetRetreatPoint(out Vector3 point)
        {
            const int retreatSearchSamples = 10;
            float botHeight = bot.controller.characterHeight;
            Vector3 position = bot.transform.position;
            Vector3 playerDelta = position - player.position;
            playerDelta.y = 0;

            Vector3 playerDeltaRight = Vector3.Cross(playerDelta.normalized, Vector3.up);
            float retreatAtDistance = stateConfig.retreatDistance - playerDelta.magnitude;
            float axisStep = Mathf.Deg2Rad * 180.0f / retreatSearchSamples;

            // first of all try playerDelta direction
            Ray ray1 = new Ray(position + Vector3.up * (botHeight * 0.5f), playerDelta);
            Ray ray2 = new Ray(position - Vector3.up * (botHeight * 0.499f), playerDelta);

            if (!Physics.Raycast(ray1, retreatAtDistance, bot.controller.visibleObjectsMask)
                || !Physics.Raycast(ray2, retreatAtDistance, bot.controller.visibleObjectsMask))
            {
                point = position + playerDelta.normalized * retreatAtDistance;
                return true;
            }

            playerDelta.Normalize();
            for (int i = 0; i < retreatSearchSamples; i++)
            {
                float t = i * axisStep;

                float x = Mathf.Sin(t);
                float y = Mathf.Cos(t);
                Vector3 direction = playerDelta * y + playerDeltaRight * x;
                ray1 = new Ray(position + Vector3.up * (botHeight * 0.5f), direction);
                ray2 = new Ray(position - Vector3.up * (botHeight * 0.499f), direction);
                if (!Physics.Raycast(ray1, retreatAtDistance, bot.controller.visibleObjectsMask)
                    || !Physics.Raycast(ray2, retreatAtDistance, bot.controller.visibleObjectsMask))
                {
                    point = position + direction * retreatAtDistance;
                    return true;
                }
            }

            point = Vector3.zero;
            return false;
        }
    }
}