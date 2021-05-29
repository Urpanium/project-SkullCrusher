using System;
using System.Collections.Generic;
using System.Linq;
using Level.Gen.Util;
using Level.Gen.VisualLayer;
using WaveFunctionCollapse3D.PathLayer.Snapshots;

namespace Level.Gen.PathLayer.PathGeneration
{
    public class PathGenerator
    {
        public PathGeneratorConfig Config { get; private set; }

        /*
         * first prototype - start node
         * last prototype - end node
         */

        public List<Prototype> MustSpawnPrototypes { get; set; }

        public List<Prototype> CanSpawnPrototypes { get; set; }


        private Queue<Prototype> currentMustSpawnPrototypesQueue;
        private List<Dector3> currentEntries;
        private List<Dector3> openEntries;
        // eating too much memory
        private Map3<PathNode> map;
        private List<PathGenerationSnapshot> history;
        private int currentPathLength;

        private int currentMustSpawnOffset;
        private int currentCanSpawnOffset;

        private Random random;

        public PathGenerator(PathGeneratorConfig config, List<Prototype> mustSpawnPrototypes,
            List<Prototype> canSpawnPrototypes)
        {
            Config = config;
            
            
            if (mustSpawnPrototypes.Count < 2)
            {
                throw new Exception(
                    $"Not enough must spawn prototypes. Need at least 2, but received {mustSpawnPrototypes.Count()}");
            } 
            float mustSpawnOffset = (float) Config.minimumPathLength / mustSpawnPrototypes.Count;


            if (mustSpawnOffset < Config.minimumOffsetBetweenMustSpawnPrototypes)
            {
                throw new Exception(
                    $"Invalid config: possible lack of path length to spawn all \"MustSpawn\" prototypes:" +
                    $" сonfig offset = {Config.minimumOffsetBetweenMustSpawnPrototypes}," +
                    $" actual offset: {mustSpawnOffset}" +
                    $" (config minimum path length / prototypes count:" +
                    $" {Config.minimumPathLength} / {MustSpawnPrototypes.Count})");
            }

            int mustSpawnRoomsSizes = 0;
            foreach (Prototype prototype in mustSpawnPrototypes)
            {
                Dector3 size = prototype.Size;
                mustSpawnRoomsSizes += Math.Max(size.x, Math.Max(size.y, size.z));
            }

            int canSpawnRoomsSizes = 0;
            foreach (Prototype prototype in canSpawnPrototypes)
            {
                Dector3 size = prototype.Size;
                canSpawnRoomsSizes += Math.Max(size.x, Math.Max(size.y, size.z));
            }


            /*
             * theoretically, map can't be longer than
             * max path length + start and end points corridors + must spawn room sizes
             * double it to have possibility to go to any direction
             * theoretically
             */
            
            int maxLengthAlongAxis = (Config.maximumPathLength + Config.maximumEndPointCorridorLength
                                                               + mustSpawnRoomsSizes + canSpawnRoomsSizes) * 2;
            Dector3 mapSize = new Dector3(maxLengthAlongAxis, maxLengthAlongAxis, maxLengthAlongAxis);


            map = new Map3<PathNode>(mapSize);
            random = new Random(Config.seed);
            
            MustSpawnPrototypes = mustSpawnPrototypes.ToList();
            CanSpawnPrototypes = canSpawnPrototypes.ToList();
        }

        public Map3<PathNode> Generate()
        {
            Reset();

            /*
             * STEP 1: build main path
             */

            /*
             * this is constant decision
             */
            Prototype startPrototype = currentMustSpawnPrototypesQueue.Dequeue();
            PlacePrototype(startPrototype, Dector3.Zero);
            AddSnapshot();


            /*while (_currentPathLength < Config.MinimumPathLength
                   && _random.NextDouble() > Config.MinimumPathContinueChance &&
                   _currentPathLength < Config.MaximumPathLength)
            {
                int entryIndex = _random.Next(_openEntries.Count);
                Dector3 entry = _openEntries[entryIndex];

                int mustSpawnDistance = Config.MinimumOffsetBetweenMustSpawnPrototypes - _currentMustSpawnOffset;


                bool isMustSpawnPrototypePlacingPossible =
                    _currentMustSpawnOffset > Config.MinimumOffsetBetweenMustSpawnPrototypes;

                /*bool isCanSpawnPrototypePlacingPossible =
                    _currentCanSpawnOffset > Config.MinimumOffsetBetweenCanSpawnPrototypes;#1#

                if (isMustSpawnPrototypePlacingPossible)
                {
                    Prototype mustSpawnPrototype = _currentMustSpawnPrototypesQueue.Dequeue();
                    int rotation;
                    Dector3 minPoint;
                    if (TryFitPrototype(entry, mustSpawnPrototype, out rotation, out minPoint))
                    {
                        mustSpawnPrototype.Rotation = rotation;
                        PlacePrototype(mustSpawnPrototype, minPoint);
                    }
                }

                /*
                 * choose a random entry + 
                 * make corridor if possible
                 * if not, choose another entry
                 * if not, try rollback last changes
                 * update open entries
                 *
                 * until the loop breaks
                 #1#
                AddSnapshot();
            }*/

            /*
             * STEP 2: resolve open entries
             */

            /*
             * take entry
             * check if it opened
             * make room/corridor/dead end
             * resolve intersections
             */
            throw new NotImplementedException();
        }


        private ulong GetDecisionsCount()
        {
            ulong decisions = 0;
            foreach (var entry in currentEntries)
            {
                Prototype currentPrototype = currentMustSpawnPrototypesQueue.Peek();
                if (CanFitPrototype(entry, currentPrototype))
                {
                    decisions++;
                }

                Dector3 entryDirection = GetEntryDirection(entry);
                int entryDirectionIndex = Dector3.GetDirectionIndex(entryDirection);
                int maxLength = Config.MaximumCorridorsLengths[entryDirectionIndex];
                int maxWidth = Config.MaximumCorridorsWidths[entryDirectionIndex];
                int maxHeight = Config.MaximumCorridorsHeights[entryDirectionIndex];

                int minLength = Config.MinimumCorridorsLengths[entryDirectionIndex];
                for (int length = minLength; length < maxLength; length++)
                {
                    for (int width = 1; width < maxWidth; width++)
                    {
                        for (int height = 1; height < maxHeight; height++)
                        {
                            if (CanFitCorridor(entry, entry + entryDirection * length, width, height))
                            {
                                decisions++;
                            }
                        }
                    }
                }
            }

            return decisions;
        }

        private PathGenerationDecision GenerateDecision()
        {
            PathGenerationDecision decision = new PathGenerationDecision();
            if (currentMustSpawnOffset > Config.minimumOffsetBetweenMustSpawnPrototypes)
            {
                // place prototype 
            }

            int distanceToNearestMustSpawnPrototype =
                Config.minimumOffsetBetweenMustSpawnPrototypes - currentMustSpawnOffset;
            int prototypesPlaced = MustSpawnPrototypes.Count - currentMustSpawnPrototypesQueue.Count;
            /*
             * frequency?
             */
            float placingFrequency = (float) currentPathLength / prototypesPlaced;
            float placingPlan = (float) Config.minimumPathLength / MustSpawnPrototypes.Count;
            Dector3 entry = currentEntries[currentEntries.Count];
            Dector3 entryDirection = GetEntryDirection(entry);

            if (placingFrequency < placingPlan)
            {
                /*
                 * need to increase placing intensity
                 */
                int minLength = Config.MinimumCorridorsLengths[Dector3.GetDirectionIndex(entryDirection)];
                int corridorLength = Math.Min(minLength, distanceToNearestMustSpawnPrototype);
                int corridorWidth =
                    random.Next(Config.MaximumCorridorsWidths[Dector3.GetDirectionIndex(entryDirection)]);
                int corridorHeight =
                    random.Next(Config.MaximumCorridorsHeights[Dector3.GetDirectionIndex(entryDirection)]);
                while (!CanFitCorridor(entry, entry + entryDirection * corridorLength, corridorWidth, corridorHeight))
                {
                    if (CanFitCorridor(entry, entry + entryDirection * corridorLength, corridorWidth - 1,
                        corridorHeight))
                    {
                        corridorWidth--;
                        break;
                    }


                    if (CanFitCorridor(entry, entry + entryDirection * corridorLength, corridorWidth,
                        corridorHeight - 1))
                    {
                        corridorHeight--;
                        break;
                    }
                    
                    corridorWidth--;
                    corridorHeight--;
                }

                if (corridorLength > 0)
                {
                    decision.GenerationStructure = GenerationStructure.Corridor;
                    decision.Length = corridorLength;
                    decision.Direction = entryDirection;
                    decision.Point = entry;
                    decision.Height = corridorHeight;
                    decision.Width = corridorWidth;
                    
                    return decision;
                }

                Dector3 minPoint;
                int rotation;
                if (TryFitPrototype(entry, currentMustSpawnPrototypesQueue.Peek(), out rotation, out minPoint))
                {
                    decision.GenerationStructure = GenerationStructure.MustSpawn;
                    decision.Point = minPoint;
                    decision.Prototype = currentMustSpawnPrototypesQueue.Dequeue();
                    decision.Rotation = rotation;
                    
                    return decision;
                }
                
                

            }
            RollbackToLastSnapshot();
            throw new NotImplementedException();
        }


        private void PlacePrototype(Prototype prototype, Dector3 minPoint)
        {
            List<Dector3> socketsWithOffset = new List<Dector3>();
            foreach (Dector3 socketPosition in prototype.SocketsPositions)
            {
                Dector3 positionWithOffset = minPoint + socketPosition;
                socketsWithOffset.Add(positionWithOffset);
                if (IsEntryOpen(positionWithOffset))
                    openEntries.Add(positionWithOffset);
            }

            PlaceRoom(socketsWithOffset, minPoint, minPoint + prototype.Size);
            AddSnapshot();

            currentPathLength++;
            currentMustSpawnOffset++;
            currentCanSpawnOffset++;

            if (MustSpawnPrototypes.Contains(prototype))
            {
                currentMustSpawnOffset = 0;
                return;
            }

            if (!CanSpawnPrototypes.Contains(prototype))
            {
                currentCanSpawnOffset = 0;
                return;
            }

            throw new Exception(
                $"IEnumerable<T>.Contains() didn't work. reached end of PlacePrototype() method." +
                $"Position: {minPoint}, {minPoint + prototype.Size}");
        }

        private void GenerateCorridor(out Dector3 direction, out int length, out int width, out int height)
        {
            Dector3[] directions = Dector3.Directions;
            float weightSum = 0.0f;
            for (int i = 0; i < 6; i++)
            {
                weightSum += Config.PathDirectionsWeights[i];
            }

            float randomNumber = (float) random.NextDouble() * weightSum;

            int directionIndex = 0;
            for (int i = 0; i < 6; i++)
            {
                float weight = Config.PathDirectionsWeights[i];
                if (randomNumber < weight)
                {
                    directionIndex = i;
                    break;
                }

                randomNumber -= weight;
            }

            direction = Dector3.GetDirection(directionIndex);
            length = random.Next(Config.MinimumCorridorsLengths[directionIndex],
                Config.MaximumCorridorsLengths[directionIndex]);

            width = random.Next(1, Config.MaximumCorridorsWidths[directionIndex]);
            height = random.Next(1, Config.MaximumCorridorsHeights[directionIndex]);
        }

        private (Dector3, Dector3) GetFreeSpace(Dector3 entry, Dector3 maxSize = null)
        {
            int[] extensions = {0, 0, 0, 0, 0, 0};
            while (true)
            {
                bool grew = false;
                foreach (Dector3 direction in Dector3.Directions)
                {
                    if (IsExtendedSpaceFree(entry, extensions, direction))
                    {
                        grew = true;
                        extensions[Dector3.GetDirectionIndex(direction)] += 1;
                    }
                }

                if (!grew)
                    break;
            }

            return GetCuboidByExtensions(entry, extensions);
        }

        private bool IsExtendedSpaceFree(Dector3 entry, int[] extensions, Dector3 direction)
        {
            /*
             * calculate current cuboid points
             */
            (Dector3, Dector3) cuboid = GetCuboidByExtensions(entry, extensions);

            Dector3 from = cuboid.Item1;
            Dector3 to = cuboid.Item2;

            /*
             * extend cuboid in the given direction
             */
            int directionIndex = Dector3.GetDirectionIndex(direction);

            if (directionIndex == Dector3.DownIndex
                || directionIndex == Dector3.BackIndex
                || directionIndex == Dector3.LeftIndex)
            {
                from += direction;
            }
            else
            {
                to += direction;
            }

            /*
             * check if it fits
             */
            return CanFitCuboid(from, to);
        }

        /*
         * tries to find rotation and position, in which prototype will fit to the entry
         * only entry, because we are on step 1
         */
        private bool TryFitPrototype(Dector3 entry, Prototype prototype, out int rotation, out Dector3 minPoint)
        {
            Dector3 entryDirection = GetEntryDirection(entry);
            Dector3 touchPoint = entry + entryDirection;

            foreach (Dector3 prototypeEntry in prototype.SocketsPositions)
            {
                Dector3 prototypeMinPoint = touchPoint - prototypeEntry;
                for (int r = 0; r < 4; r++)
                {
                    Dector3 prototypeRotatedSize = prototype.Size.Rotated(r);
                    if (CanFitCuboid(prototypeMinPoint, prototypeMinPoint + prototypeRotatedSize))
                    {
                        rotation = r;
                        minPoint = prototypeMinPoint;
                        return true;
                    }
                }
            }

            rotation = -1;
            minPoint = new Dector3();
            return false;
        }

        private bool CanFitPrototype(Dector3 entry, Prototype prototype)
        {
            Dector3 entryDirection = GetEntryDirection(entry);
            Dector3 touchPoint = entry + entryDirection;

            foreach (Dector3 prototypeEntry in prototype.SocketsPositions)
            {
                Dector3 prototypeMinPoint = touchPoint - prototypeEntry;
                for (int r = 0; r < 4; r++)
                {
                    Dector3 prototypeRotatedSize = prototype.Size.Rotated(r);
                    if (CanFitCuboid(prototypeMinPoint, prototypeMinPoint + prototypeRotatedSize))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CanFitRoom(Dector3 point1, Dector3 point2)
        {
            return CanFitCuboid(point1, point2);
        }

        private void PlaceRoom(List<Dector3> entries, Dector3 point1, Dector3 point2,
            bool checkPointsOfCuboidOnBorder = false)
        {
            (Dector3, Dector3) minAndMax = Dector3.ToMinAndMax(point1, point2);
            Dector3 from = minAndMax.Item1;
            Dector3 to = minAndMax.Item2;

            for (int x = from.x; x < to.x; x++)
            {
                for (int y = from.y; y < to.y; y++)
                {
                    for (int z = from.z; z < to.z; z++)
                    {
                        Dector3 currentPosition = new Dector3(x, y, z);
                        PathNode roomNode = new PathNode(currentPosition);
                        /*
                         * TODO: again, wrong calculations, need a rework
                         * or not?
                         * 2:49 22.05 I'm physically not able to understand this
                         * looks like it will work with start node without any changes
                         * but keep eye on this
                         */

                        roomNode.AllowGoToAnyDirection();


                        /*
                         * so we checking every node before installation
                         */
                        foreach (Dector3 direction in Dector3.Directions)
                        {
                            Dector3 nearPosition = currentPosition + direction;

                            /*
                             * if node is on the border
                             * 
                             * and not outside of map (which theoretically impossible
                             * but who know what shit can happen)
                             *
                             * and if this position is entry
                             */

                            bool onBorder = IsPointOnBorderOfCuboid(nearPosition, point1, point2);


                            bool positionIsValid = map.IsValidPosition(nearPosition);
                            bool isEntry = entries.Contains(nearPosition) ||
                                           checkPointsOfCuboidOnBorder && entries.Contains(currentPosition);

                            roomNode.SetDirectionAccess(Dector3.GetDirectionIndex(direction),
                                positionIsValid && onBorder && isEntry);
                        }

                        map.SetTile(x, y, z, roomNode);
                    }
                }
            }
        }

        private void PlaceCorridor(Dector3 from, Dector3 to, int width, int height)
        {
            if (width % 2 == 0 || height % 2 == 0)
            {
                throw new Exception($"Even widths and heights are not allowed: ({width}; {height})");
            }

            if (!Dector3.IsDifferentOnlyByOneAxis(from, to))
            {
                throw new Exception($"Attempt to make diagonal corridor along several axis: {from} {to}");
            }
        }

        private bool CanFitCorridor(Dector3 from, Dector3 to, int width, int height)
        {
            Dector3 direction = (to - from).ToOne();

            Dector3 up;
            Dector3 right;

            int directionIndex = Dector3.GetDirectionIndex(direction);
            if (directionIndex == -1)
            {
                throw new Exception($"Invalid direction: {direction}, from {from} to {to}");
            }

            if (directionIndex < 1)
            {
                up = Dector3.Forward;
                right = Dector3.Right;
            }
            else
            {
                up = Dector3.Up;
                right = Dector3.GetDirection(2 + (directionIndex - 2 + 1) % 4);
            }

            Dector3 point1 = from + up + right;
            Dector3 point2 = to - up - right;

            return CanFitCuboid(point1, point2);
        }

        private bool IsEntryOpen(Dector3 entryPosition)
        {
            PathNode entry = map.GetTile(entryPosition);
            for (int i = 0; i < Dector3.Directions.Length; i++)
            {
                Dector3 direction = Dector3.Directions[i];
                Dector3 nearPosition = entryPosition + direction;

                if (entry.GetDirectionAccess(i) && nearPosition == null)
                    return true;
            }

            return false;
        }

        private Dector3 GetEntryDirection(Dector3 entry)
        {
            foreach (Dector3 direction in Dector3.Directions)
            {
                if (map.GetTile(entry + direction) != null && map.GetTile(entry - direction) == null)
                {
                    return direction;
                }
            }

            /*
             * TODO: 
             */
            return Dector3.Directions[random.Next(Dector3.Directions.Length)];
            //return Dector3.Down;
        }

        private void RollbackToLastSnapshot()
        {
            PathGenerationSnapshot s = history[history.Count - 1];
            while (s.RestoresCount > 0)
            {
                history.RemoveAt(history.Count - 1);
                s = history[history.Count - 1];
            }

            s.RestoresCount++;
            currentMustSpawnPrototypesQueue = s.CurrentMustSpawnPrototypesQueue;
            currentEntries = s.CurrentEntries;
            openEntries = s.OpenEntries;
            map = s.Map;
            currentPathLength = s.CurrentPathLength;
            currentMustSpawnOffset = s.CurrentMustSpawnOffset;
            currentCanSpawnOffset = s.CurrentCanSpawnOffset;
            /*
             * change random seed and hope that
             * everything will be fine
             * right?
             */
            random = new Random(random.Next(int.MaxValue - s.RestoresCount - Config.seed)
                                 + s.RestoresCount + Config.seed);
            //_random = s.Random;
        }

        private void AddSnapshot()
        {
            history.Add(new PathGenerationSnapshot
            {
                CurrentMustSpawnPrototypesQueue = currentMustSpawnPrototypesQueue,
                CurrentEntries = currentEntries,
                OpenEntries = openEntries,
                Map = map,
                CurrentPathLength = currentPathLength,
                CurrentMustSpawnOffset = currentMustSpawnOffset,
                CurrentCanSpawnOffset = currentCanSpawnOffset,
                Random = random,
                RestoresCount = 0
            });
        }


        /*
         * ================================================================
         * C U B O I D   S C A R Y   F U C K I N G   S H I T
         * ================================================================
         */


        private bool CanFitCuboid(Dector3 point1, Dector3 point2)
        {
            (Dector3, Dector3) minAndMax = Dector3.ToMinAndMax(point1, point2);
            Dector3 from = minAndMax.Item1;
            Dector3 to = minAndMax.Item2;

            if (!map.IsValidPosition(from) || !map.IsValidPosition(to))
            {
                throw new Exception($"Invalid cuboid coordinates: {from} {to}");
            }

            for (int x = from.x; x < to.x; x++)
            {
                for (int y = from.y; y < to.y; y++)
                {
                    for (int z = from.z; z < to.z; z++)
                    {
                        if (map.GetTile(x, y, z) != null)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private (Dector3, Dector3) GetCuboidByExtensions(Dector3 entry, int[] extensions)
        {
            Dector3 from = entry; // down, back, left
            Dector3 to = entry; // up, forward, right

            from += Dector3.Down * extensions[Dector3.DownIndex]
                    + Dector3.Back * extensions[Dector3.BackIndex]
                    + Dector3.Left * extensions[Dector3.LeftIndex];

            to += Dector3.Up * extensions[Dector3.UpIndex]
                  + Dector3.Forward * extensions[Dector3.ForwardIndex]
                  + Dector3.Right * extensions[Dector3.RightIndex];
            return (from, to);
        }

        private bool IsEntriesOnBorderOfCuboid(IEnumerable<Dector3> entries, Dector3 point1, Dector3 point2)
        {
            foreach (Dector3 entry in entries)
            {
                if (
                    entry.x == point1.x
                    || entry.y == point1.y
                    || entry.z == point1.z
                    || entry.x == point2.x
                    || entry.y == point2.y
                    || entry.z == point2.z
                )
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsPointOnBorderOfCuboid(Dector3 entry, Dector3 point1, Dector3 point2)
        {
            return entry.x == point1.x
                   || entry.y == point1.y
                   || entry.z == point1.z
                   || entry.x == point2.x
                   || entry.y == point2.y
                   || entry.z == point2.z;
        }

        private bool IsInsideCuboid(Dector3 position, Dector3 cuboidPoint1, Dector3 cuboidPoint2, bool strict = true)
        {
            (Dector3, Dector3) minAndMax = Dector3.ToMinAndMax(cuboidPoint1, cuboidPoint2);
            Dector3 from = minAndMax.Item1;
            Dector3 to = minAndMax.Item2;
            if (strict)
            {
                return position.x > from.x && position.x < to.x
                                           && position.y > from.y && position.y < to.y
                                           && position.z > from.z && position.z < to.z;
            }

            return position.x >= from.x && position.x <= to.x
                                        && position.y >= from.y && position.y <= to.y
                                        && position.z >= from.z && position.z <= to.z;
        }


        /*
         * Tries to find a cuboid that will contain all entries on it's border
         */
        private bool TryGetCuboidFromEntries(IEnumerable<Dector3> entries, out Dector3 point1, out Dector3 point2)
        {
            entries = entries.ToList();

            if (!entries.Any())
                throw new Exception("No entries were provided");
            Dector3 minBound = entries.First();
            Dector3 maxBound = entries.First();

            foreach (Dector3 entry in entries)
            {
                if (entry.x < minBound.x)
                    minBound.x = entry.x;

                if (entry.y < minBound.y)
                    minBound.y = entry.y;

                if (entry.z < minBound.z)
                    minBound.z = entry.z;


                if (entry.x > maxBound.x)
                    maxBound.x = entry.x;

                if (entry.y > maxBound.y)
                    maxBound.y = entry.y;

                if (entry.z > maxBound.z)
                    maxBound.z = entry.z;
            }

            bool entriesOnBorder = IsEntriesOnBorderOfCuboid(entries, minBound, maxBound);
            if (!entriesOnBorder)
            {
                point1 = new Dector3();
                point2 = new Dector3();
                return false;
            }

            point1 = minBound;
            point2 = maxBound;
            return true;
        }


        /*
        * ================================================================
        * R E S E T
        * ================================================================
        */

        private void Reset()
        {
            random = new Random(Config.seed);
            map.Clear();
            /*
             * )
             */
            history.Clear();
            currentPathLength = 0;
            currentCanSpawnOffset = Config.minimumOffsetBetweenMustSpawnPrototypes;
            currentMustSpawnOffset = Config.minimumOffsetBetweenCanSpawnPrototypes;
            currentMustSpawnPrototypesQueue = (Queue<Prototype>) MustSpawnPrototypes.AsEnumerable();
            openEntries = new List<Dector3>();
        }
    }
}