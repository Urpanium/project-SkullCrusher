using AI.Classes.States.Configs;
using Level.Covers;
using Preferences;
using UnityEngine;

namespace AI.Classes.States
{
    public class TakeCoverState : AiState
    {
        private TakeCoverStateConfig stateConfig;

        private CoverManager coverManager;
        private bool takingCoverForReloading;
        private bool pickedCover;

        public TakeCoverState(AiStateConfig config, AiBot bot, Transform player): base(config, bot, player)
        {
            name = TakeCoverState;
            stateConfig = (TakeCoverStateConfig) config;
            GameObject globalController = GameObject.Find(Settings.GameObjects.GlobalController);
            coverManager = globalController.GetComponent<CoverManager>();
            takingCoverForReloading = bot.IsNeedToReload();
            
        }

        public override void Update()
        {
            if (!pickedCover)
            {
                Vector3 playerDirection = (player.position - bot.transform.position).normalized;
                Vector3 coverPosition = coverManager.GetNearestCoverPosition(bot.transform.position, playerDirection,
                    stateConfig.angleThreshold);
                bot.controller.GoTo(coverPosition);
                pickedCover = true;
            }
            
            if (pickedCover && bot.controller.IsArrivedAtTargetPosition())
            {
                // TODO: predict reloading to start searching for cover earlier
                if (takingCoverForReloading)
                {
                    bot.ReloadWeapon();
                }
            }
        }

        public override string TransitionCheck()
        {
            
        }
    }
}