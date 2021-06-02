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

        private PathMap map;
        private Random random;

        private int currentPathLength;

        private List<PathPrototype> currentMustSpawnPrototypes;
        private int currentMustSpawnOffset;

        private List<PathPrototype> currentCanSpawnPrototypes;
        private int currentCanSpawnOffset;

        private CorridorGenerator corridorGenerator;
        private RoomGenerator roomGenerator;
        private PrototypeGenerator prototypeGenerator;

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
            return map;
        }

        private PathDecision Decide(int decisionsCount = 0, PathDecision previousDecision = null)
        {
            Random decisionRandom = new Random(config.seed);
            int entryIndex = (decisionRandom.Next(currentEntries.Count) + decisionsCount) % currentEntries.Count;
            Dector3 entry = currentEntries[entryIndex];


            PathDecision decision = new PathDecision();


            /*
             * if we have some prototypes that need to be placed
             */
            if (currentMustSpawnPrototypes.Count > 0)
            {
                
                /*
                 * TODO: end placing prototype, need some work, but enough for now
                 */
                if ((debugMode || currentMustSpawnPrototypes.Count == 1)
                    && currentPathLength > config.minimumPathLength 
                    && currentPathLength < config.maximumPathLength)
                {
                    if (!(random.NextDouble() < config.minimumPathContinueChance))
                    {
                        PathPrototype prototype = currentMustSpawnPrototypes[0];

                        if (prototypeGenerator.TryFitPrototype(map, prototype, entry, out Dector3 minPoint,
                            out int rotation))
                        {
                            decision.type = PathDecisionType.Prototype;
                            decision.entry = minPoint;
                            decision.rotation = rotation;
                            decision.prototypeIndex = mustSpawnPrototypes.IndexOf(prototype);
                            return decision;
                        }
                    }
                }

                int distanceToPrototype = config.minimumOffsetBetweenMustSpawnPrototypes - currentMustSpawnOffset;

                float pathProgress = currentPathLength / ((config.minimumPathLength + config.maximumPathLength) * 0.5f);
                float prototypeProgress = (float) (mustSpawnPrototypes.Count - currentMustSpawnPrototypes.Count) /
                                          mustSpawnPrototypes.Count;
                
                
                
                /*
                 * if we passed more tiles than must spawn offset
                 */

                if (distanceToPrototype < 0)
                {
                    if (pathProgress > prototypeProgress)
                    {
                        /*
                         * TODO: place must spawn prototype
                         */
                        //decision.type = PathDecisionType.Prototype;
                        PathPrototype prototype = currentMustSpawnPrototypes[0];

                        if (prototypeGenerator.TryFitPrototype(map, prototype, entry, out Dector3 minPoint,
                            out int rotation))
                        {
                            decision.type = PathDecisionType.Prototype;
                            decision.entry = minPoint;
                            decision.rotation = rotation;
                            decision.prototypeIndex = mustSpawnPrototypes.IndexOf(prototype);
                            return decision;
                        }
                    }
                    else
                    {
                        /*
                         * next corridor will skip offset anyway
                         */

                        /*
                         * boy next corridor
                         */

                        /*
                         * i don't remember what exactly i wanted to do here,
                         * for now i'll just leave it
                         */

                        /*
                        int minCorridorLength = config.MinimumCorridorsLengths[0];
                        foreach (var corridorLength in config.MinimumCorridorsLengths)
                        {
                            if (minCorridorLength > corridorLength)
                                minCorridorLength = corridorLength;
                        }
    
                        int nextPathLength = currentPathLength + minCorridorLength;
                        float nextPathProgress = nextPathLength / ((config.minimumPathLength + config.maximumPathLength) * 0.5f);
                        float nextPrototypeProgress = (float) (mustSpawnPrototypes.Count - currentMustSpawnPrototypes.Count) /
                                                  mustSpawnPrototypes.Count;*/
                    }
                }
            }
            

            return decision;
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
                corridorsSizes += Dector3.Directions[i] * (short) Math.Round(multiplier * safetyMultiplier + 0.4f);
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
            prototypeGenerator = new PrototypeGenerator(config);

            map = new PathMap(GetMapSize());

            currentEntries = new List<Dector3>();
        }
    }
}