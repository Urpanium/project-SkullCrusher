using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WaveFunctionCollapse3D.PathLayer.Snapshots;
using WaveFunctionCollapse3D.Util;
using WaveFunctionCollapse3D.VisualLayer;

namespace WaveFunctionCollapse3D.PathLayer.PathGeneration
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


        private Queue<Prototype> _currentMustSpawnPrototypesQueue;
        private List<Dector3> _currentEntries;
        private List<Dector3> _openEntries;
        private Map3<PathNode> _map;
        private List<PathGenerationSnapshot> _history;
        private int _currentPathLength;

        private int _currentMustSpawnOffset;
        private int _currentCanSpawnOffset;

        private Random _random;

        public PathGenerator(PathGeneratorConfig config, IEnumerable<Prototype> mustSpawnPrototypes,
            IEnumerable<Prototype> canSpawnPrototypes)
        {
            float mustSpawnOffset = (float) Config.MinimumPathLength / MustSpawnPrototypes.Count;


            if (mustSpawnOffset < Config.MinimumOffsetBetweenMustSpawnPrototypes)
            {
                throw new Exception(
                    $"Invalid config: possible lack of path length to spawn all \"MustSpawn\" prototypes:" +
                    $" сonfig offset = {Config.MinimumOffsetBetweenMustSpawnPrototypes}," +
                    $" actual offset: {mustSpawnOffset}" +
                    $" (config minimum path length / prototypes count:" +
                    $" {Config.MinimumPathLength} / {MustSpawnPrototypes.Count})");
            }

            int mustSpawnRoomsSizes = MustSpawnPrototypes.Select(mustSpawnPrototype => mustSpawnPrototype.Size)
                .Select(size => Math.Max(size.X, Math.Max(size.Y, size.Z))).Sum();

            int canSpawnRoomSizes = CanSpawnPrototypes.Select(canSpawnPrototype => canSpawnPrototype.Size)
                .Select(size => Math.Max(size.X, Math.Max(size.Y, size.Z))).Sum();


            /*
             * theoretically, map can't be longer than
             * max path length + start and end points corridors + must spawn room sizes
             * double it to have possibility to go to any direction
             * theoretically
             */
            int maxLengthAlongAxis = (Config.MaximumPathLength + Config.MaximumEndPointCorridorLength
                                                               + mustSpawnRoomsSizes + canSpawnRoomSizes) * 2;
            Dector3 mapSize = new Dector3(maxLengthAlongAxis, maxLengthAlongAxis, maxLengthAlongAxis);


            _map = new Map3<PathNode>(mapSize);
            _random = new Random(Config.Seed);
            Config = config;
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
            Prototype startPrototype = _currentMustSpawnPrototypesQueue.Dequeue();
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
            foreach (var entry in _currentEntries)
            {
                Prototype currentPrototype = _currentMustSpawnPrototypesQueue.Peek();
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
            if (_currentMustSpawnOffset > Config.MinimumOffsetBetweenMustSpawnPrototypes)
            {
                // place prototype 
            }

            int distanceToNearestMustSpawnPrototype =
                Config.MinimumOffsetBetweenMustSpawnPrototypes - _currentMustSpawnOffset;
            int prototypesPlaced = MustSpawnPrototypes.Count - _currentMustSpawnPrototypesQueue.Count;
            /*
             * frequency?
             */
            float placingFrequency = (float) _currentPathLength / prototypesPlaced;
            float placingPlan = (float) Config.MinimumPathLength / MustSpawnPrototypes.Count;
            Dector3 entry = _currentEntries[_currentEntries.Count];
            Dector3 entryDirection = GetEntryDirection(entry);

            if (placingFrequency < placingPlan)
            {
                /*
                 * need to increase placing intensity
                 */
                int minLength = Config.MinimumCorridorsLengths[Dector3.GetDirectionIndex(entryDirection)];
                int corridorLength = Math.Min(minLength, distanceToNearestMustSpawnPrototype);
                int corridorWidth =
                    _random.Next(Config.MaximumCorridorsWidths[Dector3.GetDirectionIndex(entryDirection)]);
                int corridorHeight =
                    _random.Next(Config.MaximumCorridorsHeights[Dector3.GetDirectionIndex(entryDirection)]);
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
                if (TryFitPrototype(entry, _currentMustSpawnPrototypesQueue.Peek(), out rotation, out minPoint))
                {
                    decision.GenerationStructure = GenerationStructure.MustSpawn;
                    decision.Point = minPoint;
                    decision.Prototype = _currentMustSpawnPrototypesQueue.Dequeue();
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
                    _openEntries.Add(positionWithOffset);
            }

            PlaceRoom(socketsWithOffset, minPoint, minPoint + prototype.Size);
            AddSnapshot();

            _currentPathLength++;
            _currentMustSpawnOffset++;
            _currentCanSpawnOffset++;

            if (MustSpawnPrototypes.Contains(prototype))
            {
                _currentMustSpawnOffset = 0;
                return;
            }

            if (!CanSpawnPrototypes.Contains(prototype))
            {
                _currentCanSpawnOffset = 0;
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

            float randomNumber = (float) _random.NextDouble() * weightSum;

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
            length = _random.Next(Config.MinimumCorridorsLengths[directionIndex],
                Config.MaximumCorridorsLengths[directionIndex]);

            width = _random.Next(1, Config.MaximumCorridorsWidths[directionIndex]);
            height = _random.Next(1, Config.MaximumCorridorsHeights[directionIndex]);
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

            for (int x = from.X; x < to.X; x++)
            {
                for (int y = from.Y; y < to.Y; y++)
                {
                    for (int z = from.Z; z < to.Z; z++)
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


                            bool positionIsValid = _map.IsValidPosition(nearPosition);
                            bool isEntry = entries.Contains(nearPosition) ||
                                           checkPointsOfCuboidOnBorder && entries.Contains(currentPosition);

                            roomNode.SetDirectionAccess(Dector3.GetDirectionIndex(direction),
                                positionIsValid && onBorder && isEntry);
                        }

                        _map.SetTile(x, y, z, roomNode);
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
            PathNode entry = _map.GetTile(entryPosition);
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
                if (_map.GetTile(entry + direction) != null && _map.GetTile(entry - direction) == null)
                {
                    return direction;
                }
            }

            /*
             * TODO: 
             */
            return Dector3.Directions[_random.Next(Dector3.Directions.Length)];
            //return Dector3.Down;
        }

        private void RollbackToLastSnapshot()
        {
            PathGenerationSnapshot s = _history[_history.Count - 1];
            while (s.RestoresCount > 0)
            {
                _history.RemoveAt(_history.Count - 1);
                s = _history[_history.Count - 1];
            }

            s.RestoresCount++;
            _currentMustSpawnPrototypesQueue = s.CurrentMustSpawnPrototypesQueue;
            _currentEntries = s.CurrentEntries;
            _openEntries = s.OpenEntries;
            _map = s.Map;
            _currentPathLength = s.CurrentPathLength;
            _currentMustSpawnOffset = s.CurrentMustSpawnOffset;
            _currentCanSpawnOffset = s.CurrentCanSpawnOffset;
            /*
             * change random seed and hope that
             * everything will be fine
             * right?
             */
            _random = new Random(_random.Next(int.MaxValue - s.RestoresCount - Config.Seed)
                                 + s.RestoresCount + Config.Seed);
            //_random = s.Random;
        }

        private void AddSnapshot()
        {
            _history.Add(new PathGenerationSnapshot
            {
                CurrentMustSpawnPrototypesQueue = _currentMustSpawnPrototypesQueue,
                CurrentEntries = _currentEntries,
                OpenEntries = _openEntries,
                Map = _map,
                CurrentPathLength = _currentPathLength,
                CurrentMustSpawnOffset = _currentMustSpawnOffset,
                CurrentCanSpawnOffset = _currentCanSpawnOffset,
                Random = _random,
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

            if (!_map.IsValidPosition(from) || !_map.IsValidPosition(to))
            {
                throw new Exception($"Invalid cuboid coordinates: {from} {to}");
            }

            for (int x = from.X; x < to.X; x++)
            {
                for (int y = from.Y; y < to.Y; y++)
                {
                    for (int z = from.Z; z < to.Z; z++)
                    {
                        if (_map.GetTile(x, y, z) != null)
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
                    entry.X == point1.X
                    || entry.Y == point1.Y
                    || entry.Z == point1.Z
                    || entry.X == point2.X
                    || entry.Y == point2.Y
                    || entry.Z == point2.Z
                )
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsPointOnBorderOfCuboid(Dector3 entry, Dector3 point1, Dector3 point2)
        {
            return entry.X == point1.X
                   || entry.Y == point1.Y
                   || entry.Z == point1.Z
                   || entry.X == point2.X
                   || entry.Y == point2.Y
                   || entry.Z == point2.Z;
        }

        private bool IsInsideCuboid(Dector3 position, Dector3 cuboidPoint1, Dector3 cuboidPoint2, bool strict = true)
        {
            (Dector3, Dector3) minAndMax = Dector3.ToMinAndMax(cuboidPoint1, cuboidPoint2);
            Dector3 from = minAndMax.Item1;
            Dector3 to = minAndMax.Item2;
            if (strict)
            {
                return position.X > from.X && position.X < to.X
                                           && position.Y > from.Y && position.Y < to.Y
                                           && position.Z > from.Z && position.Z < to.Z;
            }

            return position.X >= from.X && position.X <= to.X
                                        && position.Y >= from.Y && position.Y <= to.Y
                                        && position.Z >= from.Z && position.Z <= to.Z;
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
                if (entry.X < minBound.X)
                    minBound.X = entry.X;

                if (entry.Y < minBound.Y)
                    minBound.Y = entry.Y;

                if (entry.Z < minBound.Z)
                    minBound.Z = entry.Z;


                if (entry.X > maxBound.X)
                    maxBound.X = entry.X;

                if (entry.Y > maxBound.Y)
                    maxBound.Y = entry.Y;

                if (entry.Z > maxBound.Z)
                    maxBound.Z = entry.Z;
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
            _random = new Random(Config.Seed);
            _map.Clear();
            /*
             * )
             */
            _history.Clear();
            _currentPathLength = 0;
            _currentCanSpawnOffset = Config.MinimumOffsetBetweenMustSpawnPrototypes;
            _currentMustSpawnOffset = Config.MinimumOffsetBetweenCanSpawnPrototypes;
            _currentMustSpawnPrototypesQueue = (Queue<Prototype>) MustSpawnPrototypes.AsEnumerable();
            _openEntries = new List<Dector3>();
        }
    }
}