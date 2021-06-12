using System;
using System.Collections.Generic;
using System.Linq;
using Level.Generation.PathLayer.Path.Decisions;
using Level.Generation.PathLayer.Path.Map;
using Level.Generation.PathLayer.Path.Prototypes;
using Level.Generation.PathLayer.Path.Snapshots;
using Level.Generation.PathLayer.Path.Structures;
using Level.Generation.PathLayer.Path.SubGenerators;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path
{
    public class PathGenerator
    {
        public PathGenerationConfig config;

        private List<PathPrototype> mustSpawnPrototypes;
        private List<PathPrototype> canSpawnPrototypes;


        private List<PathSnapshot> snapshots;

        private PathSnapshot currentSnapshot;

        private bool debugMode = false;

        public PathGenerator(PathGenerationConfig config, List<PathPrototype> mustSpawnPrototypes,
            List<PathPrototype> canSpawnPrototypes)
        {
            if (mustSpawnPrototypes == null)
                mustSpawnPrototypes = new List<PathPrototype>();
            if (canSpawnPrototypes == null)
                canSpawnPrototypes = new List<PathPrototype>();

            this.mustSpawnPrototypes = mustSpawnPrototypes;
            this.canSpawnPrototypes = canSpawnPrototypes;
            this.config = config;

            debugMode = mustSpawnPrototypes.Count < 2;

            currentSnapshot =
                PathSnapshot.Start(config, GetMapSize(), mustSpawnPrototypes.Count, canSpawnPrototypes.Count);
        }

        public PathMap GenStep()
        {
            if (snapshots.Count == 0)
            {
                EvaluateDecision(StartDecision());
                SaveSnapshot();
            }
            else
            {
                UnityEngine.Debug.Log($"snapshots: {snapshots.Count}");
                PathDecision decision = GenerateDecision();
                if (decision.type != PathDecisionType.End)
                {
                    EvaluateDecision(decision);
                    SaveSnapshot();
                }
                else
                {
                    UnityEngine.Debug.Log("End");
                }
            }

            return currentSnapshot.map;
        }

        public PathMap Generate()
        {
            PathDecision decision = StartDecision();
            EvaluateDecision(decision);

            decision = GenerateDecision();
            while (decision.type != PathDecisionType.End)
            {
                EvaluateDecision(decision);
                decision = GenerateDecision();
            }

            return currentSnapshot.map;
        }

        private PathDecision GenerateDecision(int tryNumber = 0)
        {
            /*
             * basing on a try number, try make a different decisions
             */
            PathDecision decision = new PathDecision();


            /*
             * currentEntries.Count variants
             */
            Dector3 entry = GetRandomEntry(tryNumber);
            if (entry.x == -1)
            {
                Rollback();
                return GenerateDecision(currentSnapshot.restores);
            }


            if (MustEndPath())
            {
                PathPrototype prototype = mustSpawnPrototypes[mustSpawnPrototypes.Count - 1];

                if (currentSnapshot.prototypeGenerator.TryFitPrototype(currentSnapshot.map, prototype, entry,
                    out Dector3 minPoint,
                    out int rotation))
                {
                    decision.type = PathDecisionType.End;

                    decision.entry = minPoint;
                    decision.size = prototype.size;

                    decision.prototypeId = mustSpawnPrototypes.Count - 1;
                    decision.rotation = rotation;
                    decision.newEntries = prototype.GetRotatedEntries(rotation);
                    return decision;
                }

                Rollback();
                return GenerateDecision(currentSnapshot.restores);
            }

            if (IsItTimeToSpawnAFuckingMustSpawnPrototype())
            {
                int prototypeId = mustSpawnPrototypes.Count - currentSnapshot.mustSpawnPrototypesRemain;
                PathPrototype prototype = mustSpawnPrototypes[prototypeId];

                if (currentSnapshot.prototypeGenerator.TryFitPrototype(currentSnapshot.map, prototype, entry,
                    out Dector3 minPoint,
                    out int rotation))
                {
                    decision.type = PathDecisionType.Prototype;
                    decision.entry = minPoint;

                    decision.prototypeId = prototypeId;
                    decision.rotation = rotation;
                    decision.newEntries = prototype.GetRotatedEntries(rotation);
                    return decision;
                }

                /*
                 * rollback shit, we can't build a ziggurat
                 */
                Rollback();
                return GenerateDecision(currentSnapshot.restores);
            }

            /*
             * ================================================================
             * ================= F R E E D O M    I S L A N D =================
             * ================================================================
             */
            /*
             * looks like we are free to do anything we want here
             */
            UnityEngine.Debug.Log("Freedom");
            /*
             * decide what to generate
             */
            List<float> structuresWeights = new List<float>
                {config.corridorWeight, config.prototypeWeight, config.roomWeight};
            /*
             * divide by entries to try all variants
             */

            /*
             * 3 variants 
             */
            int randomIndex = (currentSnapshot.weightedRandom.RandomWeightedIndex(structuresWeights) +
                               tryNumber / currentSnapshot.currentEntries.Count) % 3;
            UnityEngine.Debug.Log($"Random index is {randomIndex}");
            if (randomIndex == 0)
            {
                /*
                 * corridor
                 */
                Cuboid corridor =
                    currentSnapshot.corridorGenerator.Generate(currentSnapshot.map, entry, out Dector3 newEntry,
                        tryNumber);
                if (newEntry.x != -1)
                {
                    decision.type = PathDecisionType.Corridor;
                    decision.entry = corridor.position;
                    decision.size = corridor.size;
                    decision.newEntries = new List<Dector3> {newEntry};
                    return decision;
                }

                Rollback();
                return GenerateDecision(currentSnapshot.restores);
            }

            if (randomIndex == 1)
            {
                if (debugMode)
                {
                    Rollback();
                    return GenerateDecision(currentSnapshot.restores);
                }

                /*
                 * prototype (CanSpawn)
                 */
                List<float> weights = new List<float>();
                for (int i = 0; i < canSpawnPrototypes.Count; i++)
                {
                    weights.Add(canSpawnPrototypes[i].weight);
                }

                int index = currentSnapshot.weightedRandom.RandomWeightedIndex(weights);

                PathPrototype prototype = canSpawnPrototypes[index];

                if (currentSnapshot.prototypeGenerator.TryFitPrototype(currentSnapshot.map, prototype, entry,
                    out Dector3 minPoint, out int rotation))
                {
                    int prototypeId = mustSpawnPrototypes.Count + index;

                    decision.type = PathDecisionType.Prototype;
                    decision.entry = minPoint;
                    decision.size = prototype.size.Rotated(rotation);
                    decision.prototypeId = prototypeId;
                    decision.rotation = rotation;
                    decision.newEntries = prototype.GetRotatedEntries(rotation);
                    return decision;
                }

                Rollback();
                return GenerateDecision(currentSnapshot.restores);
            }

            if (randomIndex == 2)
            {
                /*
                 * room
                 */
                Cuboid room =
                    currentSnapshot.roomGenerator.Generate(currentSnapshot.map, entry, out List<Dector3> newEntries, true,
                        tryNumber);
                if (room.position.x != -1)
                {
                    decision.type = PathDecisionType.Room;
                    decision.entry = room.position;
                    decision.size = room.size;
                    decision.newEntries = newEntries;
                    return decision;
                }

                Rollback();
                return GenerateDecision(currentSnapshot.restores);
            }


            throw new Exception("Decision generation ended with no decision to generate.");
        }

        private Dector3 GetRandomEntry(int tryNumber = 0)
        {
            List<Dector3> currentEntries = currentSnapshot.currentEntries;
            if (currentEntries.Count == 0)
            {
                return new Dector3(-1, -1, -1);
            }

            Random entriesShuffleRandom = new Random(config.seed);

            for (int i = 0; i < currentEntries.Count / 2; i++)
            {
                int index1 = entriesShuffleRandom.Next(currentEntries.Count);
                int offset = entriesShuffleRandom.Next(currentEntries.Count - 1);
                int index2 = (index1 + offset) % currentEntries.Count;
                Dector3 buffer = currentEntries[index2];
                currentEntries[index2] = currentEntries[index1];
                currentEntries[index1] = buffer;
            }

            return currentEntries[tryNumber % currentEntries.Count];
        }

        private bool IsItTimeToSpawnAFuckingMustSpawnPrototype()
        {
            bool can = currentSnapshot.currentMustSpawnOffset > config.minimumOffsetBetweenMustSpawnPrototypes;

            if (!can)
                return false;

            float configMaximumSpawnRate = (float) mustSpawnPrototypes.Count / config.minimumPathLength;
            float pathSpawnRate = (float) (mustSpawnPrototypes.Count - currentSnapshot.canSpawnPrototypesRemain) /
                                  currentSnapshot.currentPathLength;

            if (pathSpawnRate < configMaximumSpawnRate)
            {
                return !CanEndPath();
            }

            /*
             * let the random decide
             */
            float value = (float) currentSnapshot.random.NextDouble();
            return value < config.mustSpawnPrototypeRate;
        }


        private bool CanEndPath()
        {
            bool isLastPrototype = currentSnapshot.mustSpawnPrototypesRemain == 1;
            bool canEnd = currentSnapshot.currentPathLength > config.minimumPathLength;
            return isLastPrototype && canEnd;
        }

        private bool MustEndPath()
        {
            int minimumCorridorLength = config.MinimumCorridorsLengths[0];
            for (int i = 1; i < config.MinimumCorridorsLengths.Length; i++)
            {
                int currentLength = config.MinimumCorridorsLengths[i];
                if (minimumCorridorLength < currentLength)
                {
                    minimumCorridorLength = currentLength;
                }
            }


            return minimumCorridorLength > config.maximumPathLength - currentSnapshot.currentPathLength;
        }


        private bool EvaluateDecision(PathDecision decision)
        {
            UnityEngine.Debug.Log($"DECISION: {decision}");
            for (int i = 0; i < decision.newEntries.Count; i++)
            {
                currentSnapshot.allEntries.Add(decision.newEntries[i]);
            }

            currentSnapshot.currentEntries = decision.newEntries;

            if (decision.type == PathDecisionType.Room
                || decision.type == PathDecisionType.Corridor)
            {
                Cuboid cuboid = Cuboid.FromPosition(decision.entry, decision.size);
                /*
                 * already checkin in generators
                 */
                /*if (CanFitCuboid(currentSnapshot.map, cuboid))
                {*/
                BuildCuboid(cuboid, true);
                currentSnapshot.currentPathLength += cuboid.size.PseudoDistance();

                return true;
                /*}*/
            }

            if (decision.type == PathDecisionType.Prototype)
            {
                PathPrototype prototype = GetPrototypeById(decision.prototypeId);
                prototype.rotation = decision.rotation;
                Cuboid cuboid = Cuboid.FromPosition(decision.entry, decision.size.Rotated(decision.rotation));

                if (CanFitCuboid(currentSnapshot.map, cuboid))
                {
                    BuildPrototype(prototype);
                    currentSnapshot.currentPathLength += cuboid.size.PseudoDistance();
                }
            }


            return false;
        }


        private PathDecision StartDecision()
        {
            PathDecision decision = new PathDecision();
            Dector3 startEntry = currentSnapshot.map.size / 2;
            if (debugMode)
            {
                Cuboid corridor =
                    currentSnapshot.corridorGenerator.Generate(currentSnapshot.map, startEntry, out var newEntry, 0);
                decision.type = PathDecisionType.Corridor;

                decision.entry = corridor.position;
                decision.to = newEntry;
                decision.size = corridor.size;

                decision.newEntries = new List<Dector3> {newEntry};
            }
            else
            {
                /*
                 * TODO: make normal start
                 */
            }

            return decision;
        }

        private PathPrototype GetPrototypeById(int id)
        {
            if (id > mustSpawnPrototypes.Count)
                return canSpawnPrototypes[id - mustSpawnPrototypes.Count];
            return mustSpawnPrototypes[id];
        }

        private void BuildCuboid(Cuboid cuboid,
            bool changeable = false)
        {
            PathMap map = currentSnapshot.map;

            Dector3 from = cuboid.position;
            Dector3 to = cuboid.To();
            for (int x = from.x; x <= to.x; x++)
            {
                for (int y = from.y; y <= to.y; y++)
                {
                    for (int z = from.z; z <= to.z; z++)
                    {
                        Dector3 position = new Dector3(x, y, z);

                        PathTile pathTile = new PathTile();

                        if (cuboid.IsInside(position))
                        {
                            pathTile.AllowGoToAnyDirection();
                            pathTile.Changeable = changeable;
                        }
                        else
                        {
                            for (int i = 0; i < Dector3.Directions.Length; i++)
                            {
                                Dector3 direction = Dector3.GetDirection(i);
                                Dector3 nearPosition = position + direction;


                                bool isValid = map.IsPositionValid(nearPosition);
                                bool isInside = cuboid.IsInside(nearPosition, false);


                                bool access = isValid && isInside;

                                if (isValid)
                                {
                                    PathTile nearTile = PathTile.FromByte(map.GetTile(nearPosition));

                                    if (currentSnapshot.allEntries.Contains(nearPosition) /*||
                                        (nearTile.Changeable 
                                        && changeable)*/ )
                                    {
                                        UnityEngine.Debug.Log($"SetDirectionAccess direction {direction * -1} index {Dector3.GetDirectionIndex(direction * -1)}");
                                        nearTile.SetDirectionAccess(
                                            Dector3.GetDirectionIndex(direction * -1),
                                            true
                                        );
                                        map.SetTile(nearPosition, nearTile);
                                        UnityEngine.Debug.Log(
                                            $"pos {nearPosition} directions: {direction} {direction * -1}");
                                        access = true;
                                    }
                                }

                                pathTile.SetDirectionAccess(
                                    i,
                                    access
                                );
                                pathTile.Changeable = changeable;
                            }
                        }

                        map.SetTile(position, pathTile);
                    }
                }
            }
        }

        private void BuildPrototype(PathPrototype prototype)
        {
            /*
             * TODO: implement
             */
            //PathMap map = currentSnapshot.map;
            UnityEngine.Debug.Log("BuildPrototype() was called, yea");
        }

        private bool CanFitCuboid(PathMap map, Cuboid cuboid)
        {
            Dector3 from = cuboid.position;
            Dector3 to = cuboid.To();

            if (
                !map.IsPositionValid(from) ||
                !map.IsPositionValid(to))
            {
                UnityEngine.Debug.Log($"Can't fit because of invalid point(s): {cuboid} (mapSize: {map.size})");
                return false;
            }


            for (int x = from.x; x < to.x; x++)
            {
                for (int y = from.y; y < to.y; y++)
                {
                    for (int z = from.z; z < to.z; z++)
                    {
                        if (!map.IsTileEmpty(x, y, z))
                        {
                            UnityEngine.Debug.Log("Can't fit because of non empty tiles");
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private Dector3 GetMapSize()
        {
            /*
             * remember: no LINQ
             */

            Dector3 prototypesSizes = new Dector3();
            Dector3 corridorsSizes = new Dector3();

            int configPathLength = config.maximumPathLength;

            float[] pathDirectionsWeightsNormalized = config.PathDirectionsWeights;
            float maxDirectionWeight = 0.01f;

            foreach (var prototype in mustSpawnPrototypes)
            {
                prototypesSizes += prototype.size;
            }

            foreach (var prototype in canSpawnPrototypes)
            {
                prototypesSizes += prototype.size;
            }


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
                float multiplier = configPathLength
                                   * config.MaximumCorridorsLengths[i]
                                   * pathDirectionsWeightsNormalized[i];

                Dector3 direction = Dector3.Directions[i];
                Dector3 dectorToAdd = new Dector3(
                    (int) Math.Round(Math.Abs(direction.x) * multiplier),
                    (int) Math.Round(Math.Abs(direction.y) * multiplier),
                    (int) Math.Round(Math.Abs(direction.z) * multiplier)
                );
                UnityEngine.Debug.Log($"Direction {direction} multiplied by {multiplier}");
                corridorsSizes += dectorToAdd;
            }


            Dector3 mapSize = corridorsSizes + prototypesSizes;

            float s = config.mapSizeSafetyMultiplier;

            mapSize = new Dector3((int) Math.Round(mapSize.x * s), (int) Math.Round(mapSize.y * s),
                (int) Math.Round(mapSize.z * s));

            UnityEngine.Debug.Log($"GENERATED MAP SIZE: {mapSize}");
            return mapSize;
        }

        private void SaveSnapshot()
        {
            currentSnapshot.restores = 0;
            snapshots.Add(currentSnapshot);
        }

        private void Rollback()
        {
            int index = snapshots.Count - 1;
            while (index > 0 && snapshots[index].restores > config.perEntryDecisionsLimit)
            {
                snapshots.RemoveAt(index);
                index--;
            }


            if (index <= 0)
            {
                //UnityEngine.Debug.Log("Returned to start");
                throw new Exception("Returned to start");
                index = 0;
            }

            /*
             * increase restores count if it is not start
             */
            currentSnapshot = snapshots[index];
            snapshots[index].restores++;
        }

        public List<Dector3> GetCurrentEntries()
        {
            return currentSnapshot.currentEntries;
        }

        public List<Dector3> GetAllEntries()
        {
            return currentSnapshot.allEntries;
        }

        public void Reset()
        {
            if (snapshots == null)
                snapshots = new List<PathSnapshot>();
            snapshots.Clear();
            currentSnapshot =
                PathSnapshot.Start(config, GetMapSize(), mustSpawnPrototypes.Count, canSpawnPrototypes.Count);
        }
    }
}