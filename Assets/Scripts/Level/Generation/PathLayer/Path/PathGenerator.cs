using System;
using System.Collections.Generic;
using Level.Generation.PathLayer.Path.Decisions;
using Level.Generation.PathLayer.Path.Prototypes;
using Level.Generation.PathLayer.Path.Snapshots;
using Level.Generation.PathLayer.Path.Structures;
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

            if (mustSpawnPrototypes == null)
                mustSpawnPrototypes = new List<PathPrototype>();
            if (canSpawnPrototypes == null)
                canSpawnPrototypes = new List<PathPrototype>();

            this.mustSpawnPrototypes = mustSpawnPrototypes;
            this.canSpawnPrototypes = canSpawnPrototypes;

            debugMode = mustSpawnPrototypes.Count < 2;

            Reset();
        }

        public PathMap Generate()
        {
            PathDecision decision = StartDecision();
            TryEvaluateDecision(decision);

            while (decision.type != PathDecisionType.End)
            {
                decision = Decide();
            }

            return map;
        }

        private PathDecision StartDecision()
        {
            PathDecision decision = new PathDecision();
            if (debugMode)
            {
                Dector3 entry = GetMapSize(config.mapSizeSafetyMultiplier) / 2;
                Dector3 to;
                Cuboid cuboid = corridorGenerator.GenerateCorridor(map, entry, out to);

                decision.type = PathDecisionType.Corridor;
                decision.entry = cuboid.position;
                decision.size = cuboid.size;
                decision.to = to;
            }
            /*
             * TODO: make non debug mode case
             */

            return decision;
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
                            decision.type = PathDecisionType.End;
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
                }
            }

            /*
             * corridor generation
             */

            /*
             * TODO: randomize between corridors and rooms
             */

            Dector3 to;
            Cuboid corridorCuboid = corridorGenerator.GenerateCorridor(map, entry, out to, decisionsCount);

            decision.type = PathDecisionType.Corridor;
            decision.entry = corridorCuboid.position;
            decision.size = corridorCuboid.size;
            decision.to = to;

            return decision;
        }

        private bool TryEvaluateDecision(PathDecision decision)
        {
            /*
             * TODO: check if can apply decision and apply if can
             * else need to try another decision
             * if all decisions are tried, rollback to last snapshot
             */
            if (decision.type == PathDecisionType.Corridor)
            {
                Cuboid cuboid = Cuboid.FromPosition(decision.entry, decision.size);
                if (CanFitCuboid(cuboid))
                {
                    AddSnapshot(decision);
                    BuildCuboid(cuboid, new List<Dector3> {decision.entry, decision.to});
                    currentPathLength += Dector3.Distance(decision.entry, decision.to);
                    currentEntries.Add(decision.to);
                    return true;
                }

                return false;
                /*
                 * rollback
                 *
                 *
                 * but not here?
                 */

                /*
                 while (history.Count > 0 && history[history.Count - 1].restores > config.perEntryDecisionsLimit)
                {
                    history.RemoveAt(history.Count - 1);
                }

                if (history.Count == 0)
                {
                    Re
                }
                */
            }

            if (decision.type == PathDecisionType.Prototype)
            {
                PathPrototype prototype;
                if (decision.prototypeIndex >= mustSpawnPrototypes.Count)
                {
                    prototype = canSpawnPrototypes[decision.prototypeIndex - mustSpawnPrototypes.Count];
                }
                else
                {
                    prototype = mustSpawnPrototypes[decision.prototypeIndex];
                }

                prototype.rotation = decision.rotation;

                Cuboid cuboid = Cuboid.FromPosition(decision.entry, decision.size.Rotated(decision.rotation));
                if (CanFitCuboid(cuboid))
                {
                    List<Dector3> rotatedEntries = prototype.GetRotatedEntries();
                    AddSnapshot(decision);
                    BuildCuboid(cuboid, rotatedEntries);
                    foreach (var localPos in rotatedEntries)
                    {
                        currentEntries.Add(cuboid.position + localPos);
                    }

                    currentPathLength += 1;
                    return true;
                }

                return false;
            }

            return false;
            throw new NotImplementedException();
        }

        private void BuildCuboid(Cuboid cuboid, List<Dector3> entries, bool checkPointsOfCuboidOnBorder = false)
        {
            Dector3 from = cuboid.position;
            Dector3 to = cuboid.To();
            for (int x = from.x; x < to.x; x++)
            {
                for (int y = from.y; y < to.y; y++)
                {
                    for (int z = from.z; z < to.z; z++)
                    {
                        Dector3 position = new Dector3(x, y, z);
                        PathTile pathTile = new PathTile();

                        foreach (Dector3 direction in Dector3.Directions)
                        {
                            Dector3 nearPosition = position + direction;

                            /*
                             * if node is on the border
                             * 
                             * and not outside of map (which theoretically impossible
                             * but who know what shit can happen)
                             *
                             * and if this position is entry
                             */

                            bool onBorder = cuboid.IsOnBorder(nearPosition);


                            map.ValidatePosition(nearPosition);
                            bool isEntry = entries.Contains(nearPosition) ||
                                           checkPointsOfCuboidOnBorder && entries.Contains(position);

                            pathTile.SetDirectionAccess(Dector3.GetDirectionIndex(direction), onBorder && isEntry);
                        }

                        map.SetTile(position, pathTile);
                    }
                }
            }
        }

        private bool CanFitCuboid(Cuboid cuboid)
        {
            Dector3 from = cuboid.position;
            Dector3 to = cuboid.To();

            map.ValidatePosition(from);
            map.ValidatePosition(to);

            for (int x = from.x; x < to.x; x++)
            {
                for (int y = from.y; y < to.y; y++)
                {
                    for (int z = from.z; z < to.z; z++)
                    {
                        if (!map.IsTileEmpty(x, y, z))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
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
            float maxDirectionWeight = 0.01f;
            foreach (var t in pathDirectionsWeightsNormalized)
            {
                if (t > maxDirectionWeight)
                    maxDirectionWeight = t;
            }

            for (int i = 0; i < pathDirectionsWeightsNormalized.Length; i++)
            {
                pathDirectionsWeightsNormalized[i] /= maxDirectionWeight;
            }


            for (int i = 0; i < 6; i++)
            {
                float multiplier = configPathLength * config.MaximumCorridorsLengths[i] *
                                   pathDirectionsWeightsNormalized[i];
                corridorsSizes += Dector3.Directions[i].WithAbsAxis() * (int) Math.Round(multiplier * safetyMultiplier + 0.4f);
            }


            Dector3 mapSize = (corridorsSizes + prototypesSizes) * 2;

            return mapSize;
        }

        private void AddSnapshot(PathDecision decision)
        {
            PathSnapshot snapshot = new PathSnapshot(map, decision);

            snapshot.random = random;

            snapshot.corridorGenerator = corridorGenerator;
            snapshot.roomGenerator = roomGenerator;
            snapshot.prototypeGenerator = prototypeGenerator;

            snapshot.mustSpawnPrototypesRemain = currentMustSpawnPrototypes.Count;
            snapshot.currentMustSpawnOffset = currentMustSpawnOffset;

            snapshot.canSpawnPrototypesRemain = currentCanSpawnPrototypes.Count;
            snapshot.currentCanSpawnOffset = currentCanSpawnOffset;

            snapshot.currentEntries = currentEntries;

            history.Add(snapshot);
        }

        private PathDecision RollbackTo(PathSnapshot snapshot)
        {
            map = snapshot.map;

            random = snapshot.random;

            corridorGenerator = snapshot.corridorGenerator;
            roomGenerator = snapshot.roomGenerator;
            prototypeGenerator = snapshot.prototypeGenerator;

            currentMustSpawnOffset = snapshot.currentMustSpawnOffset;
            currentMustSpawnPrototypes.Clear();
            for (int i = mustSpawnPrototypes.Count - snapshot.mustSpawnPrototypesRemain;
                i < mustSpawnPrototypes.Count;
                i++)
            {
                currentMustSpawnPrototypes.Add(mustSpawnPrototypes[i]);
            }

            currentCanSpawnOffset = snapshot.currentCanSpawnOffset;
            currentCanSpawnPrototypes.Clear();
            for (int i = canSpawnPrototypes.Count - snapshot.canSpawnPrototypesRemain;
                i < canSpawnPrototypes.Count;
                i++)
            {
                currentCanSpawnPrototypes.Add(canSpawnPrototypes[i]);
            }

            currentEntries = snapshot.currentEntries;
            snapshot.restores++;

            return snapshot.decision;
        }


        public void Reset()
        {
            random = new Random(config.seed);

            currentPathLength = 0;

            corridorGenerator = new CorridorGenerator(config);
            roomGenerator = new RoomGenerator(config);
            prototypeGenerator = new PrototypeGenerator(config);

            map = new PathMap(GetMapSize(config.mapSizeSafetyMultiplier));

            currentEntries = new List<Dector3>();
        }
    }
}