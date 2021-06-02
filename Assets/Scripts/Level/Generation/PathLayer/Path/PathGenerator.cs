using System;
using System.Collections.Generic;
using Level.Generation.PathLayer.Path.Decisions;
using Level.Generation.PathLayer.Path.Prototypes;
using Level.Generation.PathLayer.Path.Snapshots;
using Level.Generation.PathLayer.Path.SubGenerators;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path
{
    public class PathGenerator
    {
        public PathGeneratorConfig config;

        private List<PathPrototype> mustSpawnPrototypes;
        private List<PathPrototype> canSpawnPrototypes;

        private List<PathSnapshot> history;

        /*
         * generation stuff
         */
        private Random random;

        private int currentPathLength;

        private List<PathPrototype> currentMustSpawnPrototypes;
        private int currentMustSpawnOffset;

        private List<PathPrototype> currentCanSpawnPrototypes;
        private int currentCanSpawnOffset;

        private CorridorGenerator corridorGenerator;
        private RoomGenerator roomGenerator;

        private List<Dector3> currentEntries;

        private bool debugMode;

        public PathGenerator(
            PathGeneratorConfig config,
            List<PathPrototype> mustSpawnPrototypes,
            List<PathPrototype> canSpawnPrototypes
        )
        {
            this.config = config;
            this.mustSpawnPrototypes = mustSpawnPrototypes;
            this.canSpawnPrototypes = canSpawnPrototypes;

            debugMode = mustSpawnPrototypes.Count < 2;

            Reset();
        }

        public PathMap Generate()
        {
            Dector3 mapSize = GetMapSize();
            PathMap map = new PathMap(mapSize);


            return map;
        }

        private PathDecision Decide(int decisionsCount)
        {
            /*
             * so let's start with checking if we
             * are succeeding in our prototypes
             * placement
             */
            PathDecision decision = new PathDecision();
            
            int distanceToPrototype = config.minimumOffsetBetweenMustSpawnPrototypes - currentMustSpawnOffset;

            float pathProgress = currentPathLength / ((config.minimumPathLength + config.maximumPathLength) * 0.5f);
            float prototypeProgress = (float) (mustSpawnPrototypes.Count - currentMustSpawnPrototypes.Count) /
                                      mustSpawnPrototypes.Count;
            if (distanceToPrototype < 0)
            {
                if (pathProgress > prototypeProgress)
                {
                    /*
                     * TODO: place must spawn prototype
                     */
                    decision.type = PathDecisionType.Prototype;
                }
            }
            /*
             * TODO: make decision
             */
            throw new NotImplementedException();
        }

        private bool TryEvaluateDecision(PathDecision decision)
        {
            /*
             * TODO: check if can apply decision and apply if can
             * else need to try another decision
             * if all decisions are tried, rollback to last snapshot
             */
            throw new NotImplementedException();
        }


        private Dector3 GetMapSize(float safetyMultiplier = 2.0f)
        {
            /*
             * remember: no LINQ
             */

            Dector3 prototypesSizes = new Dector3();
            foreach (var prototype in mustSpawnPrototypes)
            {
                prototypesSizes += prototype.size;
            }

            foreach (var prototype in canSpawnPrototypes)
            {
                prototypesSizes += prototype.size;
            }

            Dector3 corridorsSizes = new Dector3();
            int configPathLength = config.maximumPathLength;
            float[] pathDirectionsWeightsNormalized = config.PathDirectionsWeights;


            for (int i = 0; i < 6; i++)
            {
                float multiplier = configPathLength * config.MaximumCorridorsLengths[i] *
                                   pathDirectionsWeightsNormalized[i];
                corridorsSizes += Dector3.Directions[i] * (int) Math.Round(multiplier * safetyMultiplier + 0.4f);
            }


            Dector3 mapSize = (corridorsSizes + prototypesSizes) * 2;

            return mapSize;
        }

        public void Reset()
        {
            random = new Random(config.seed);

            currentPathLength = 0;

            corridorGenerator = new CorridorGenerator(config);
            roomGenerator = new RoomGenerator(config);

            currentEntries = new List<Dector3>();
        }
    }
}