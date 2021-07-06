using System.Collections.Generic;
using AI.Classes.States.Configs;
using Level.Covers;
using Level.Covers.Classes;
using UnityEngine;
using Random = System.Random;


namespace AI.Classes.States
{
    public class SearchState : AiState
    {
        private SearchStateConfig stateConfig;
        private CoverManager coverManager;
        private Random random;

        private List<Vector3> positionsToCheck;

        private float currentWaitTime;

        public SearchState(AiStateConfig config, AiBot bot, Transform player) : base(config, bot, player)
        {
            name = SearchState;
            stateConfig = (SearchStateConfig) config;
            random = new Random(config.GetHashCode() + bot.config.GetHashCode());
            positionsToCheck = GetCoverListToCheck(bot.transform.position, bot.GetPlayerLastVelocity(),
                stateConfig.coverChoiceLimiter);
        }

        public override void Update()
        {
            if (currentWaitTime <= 0)
            {
            }
        }

        public override string TransitionCheck()
        {
            throw new System.NotImplementedException();
        }


        private List<Vector3> GetCoverListToCheck(Vector3 position, Vector3 direction, int limiter)
        {
            List<Vector3> result = new List<Vector3>();
            List<Cover> nearestCluster = coverManager.GetNearestClusterList(position);
            
            if (limiter < 0)
                limiter = nearestCluster.Count;
            
            // TODO: change limiter depending on difficulty
            for (int i = 0; i < Mathf.Min(nearestCluster.Count, limiter); i++)
            {
                Vector3 coverPosition = nearestCluster[i].position;
                Vector3 coverDirection = (nearestCluster[i].position - position).normalized;
                if (Vector3.Dot(direction, coverDirection) <
                    stateConfig.maxDirectionDifference)
                {
                    result.Add(coverPosition);
                }
                
            }

            return result;
        }


        private Vector3 RandomizeDirection(Vector3 direction, float difference)
        {
            float multiplier = ((float) random.NextDouble() * 2 - 1) * difference;

            Vector3 randomVector = GetRandomVector3() * 2 * multiplier;
            return (direction + randomVector).normalized;
        }

        private Vector3 GetRandomVector3()
        {
            float x = (float) random.NextDouble() * 2 - 1;
            float y = (float) random.NextDouble() * 2 - 1;
            float z = (float) random.NextDouble() * 2 - 1;
            return new Vector3(x, y, z).normalized;
        }
    }
}