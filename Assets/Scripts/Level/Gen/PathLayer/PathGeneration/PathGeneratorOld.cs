using System;
using System.Collections.Generic;
using System.Linq;
using WaveFunctionCollapse3D.Util;
using WaveFunctionCollapse3D.VisualLayer;
using WaveFunctionCollapse3D.WaveFunction;

namespace WaveFunctionCollapse3D.PathLayer.PathGeneration
{
    /*
     * so this old version of path generator
     * it was never finished
     * new was made due to it's possible better speed
     * and better understanding what the fuck is going on
     */
    
    
    // generates initial player's path
    public class PathGeneratorOld
    {
        public List<PathNode> Nodes { get; private set; }

        public PathGeneratorConfig Config { get; private set; }

        // some scripted sequences, like weapon spawn or big prototypes
        // will be spawned in listed order
        public List<Prototype> MustSpawnPrototypes { get; set; }

        // big prototypes, which can be spawned, but not for sure, so we must prepare some space for them
        public List<Prototype> CanSpawnPrototypes { get; set; }


        private int _currentPathLength;

        private int _currentMustSpawnOffset;
        private int _currentCanSpawnOffset;

        private Random _random;

        public PathGeneratorOld()
        {
            _random = new Random(Config.Seed);
        }


        public PathGeneratorOld(PathGeneratorConfig config)
        {
            Config = config;
            _random = new Random(Config.Seed);
        }

        public void Generate()
        {
            Reset();

            // place start node
            PathNode startNode = new PathNode
            {
                Position =
                {
                    X = 0,
                    Y = 0,
                    Z = 0,
                },
                Type = PathNodeType.Start
            };
            Nodes.Add(startNode);

            // generate until prefered size is reached

            /*
            PathNode currentNode = GenerateNextPathNode(startNode);
            while (_currentPathLength < Config.MinimumPathLength - 1 ||
                   (_random.NextDouble() > Config.MinimumPathContinueChance &&
                    _currentPathLength < Config.MaximumPathLength))
            {
                currentNode = GenerateNextPathNode(currentNode);
            }
            */


            // ...


            // place end node
        }

        /*
         * generates approximate corridors and rooms of map
         * 
         */
        private List<PathNode> GenerateApproximateMainPath()
        {
            int requiredPrototypesCount = MustSpawnPrototypes.Count;
            /*
             * -2 for start and end nodes
             */
            int maxLengthPerCorridor =
                (int) Math.Floor((float) (Config.MaximumPathLength - 2) / requiredPrototypesCount);

            /*
             * -2 for start and end nodes
             */
            int minLengthPerCorridor =
                (int) Math.Floor((float) (Config.MinimumPathLength - 2) / requiredPrototypesCount);


            throw new NotImplementedException();
        }

        private (Dector3, Dector3) GenerateCorridor(List<Dector3> entries)
        {
            throw new NotImplementedException();
        }

        private void GenerateRoom(List<Dector3> entries, Dector3 size = null)
        {
            Dector3 entriesCubePosition;
            Dector3 entriesCubeSize;

            // TODO: border checking or something, i forgot
            
            

            if (size == null)
            {
                //Dector3 freeSpace = IsSpaceFree();
                int width = _random.Next(Config.MinimumRoomXSize, Config.MaximumRoomXSize);
                int height = _random.Next(Config.MinimumRoomYSize, Config.MaximumRoomYSize);
                int length = _random.Next(Config.MinimumRoomZSize, Config.MaximumRoomZSize);

                size = new Dector3(width, height, length);
            }

            throw new NotImplementedException();
        }


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

        private bool IsEntriesOnBorderOfCuboid(IEnumerable<Dector3> entries, Dector3 point1, Dector3 point2)
        {
            /*Dector3 point2 = point1 + cuboidSize;*/
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

        private bool HasAlternativePaths()
        {
            // check if checkpoints have alternative path which allow to skip important map fragment
            throw new NotImplementedException();
        }

        private void PlaceRoom(Dector3 where, Dector3 size, List<Dector3> entriesLocalPositions)
        {
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    for (int z = 0; z < size.Z; z++)
                    {
                        Dector3 offset = new Dector3(x, y, z);
                        PathNode roomNode = new PathNode(where + offset);
                        roomNode.AllowGoToAnyDirection();
                        if (!entriesLocalPositions.Contains(offset))
                        {
                            if (x == 0)
                            {
                                roomNode.CanGoToLeft = false;
                            }

                            if (x == size.X - 1)
                            {
                                roomNode.CanGoToRight = false;
                            }

                            if (y == 0)
                            {
                                roomNode.CanGoToBottom = false;
                            }

                            if (y == size.Y - 1)
                            {
                                roomNode.CanGoToTop = false;
                            }

                            if (z == 0)
                            {
                                roomNode.CanGoToBack = false;
                            }

                            if (z == size.Z - 1)
                            {
                                roomNode.CanGoToForward = false;
                            }

                            Nodes.Add(roomNode);
                        }
                    }
                }
            }
        }

        private bool CanFitRoom(Dector3 where, Dector3 size)
        {
            int x1 = where.X;
            int x2 = where.X + size.X;

            int y1 = where.Y;
            int y2 = where.Y + size.Y;

            int z1 = where.Z;
            int z2 = where.Z + size.Z;

            foreach (var node in Nodes)
            {
                Dector3 p = node.Position;
                if (
                    p.X > x1 && p.X < x2 ||
                    p.Y > y1 && p.Y < y2 ||
                    p.Z > z1 && p.Z < z2
                )
                    return false;
            }

            return true;
        }

        private void PlaceCorridor(Dector3 from, Dector3 to, int width, int height)
        {
            if (width % 2 == 0 || height % 2 == 0)
            {
                throw new Exception($"Even widths and heights are not allowed: ({width}; {height})");
            }

            if (!Dector3.IsDifferentOnlyByOneAxis(from, to))
            {
                throw new Exception($"Attempt to make corridor along several axis: {from} {to}");
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

        private PathNode GenerateNextPathNode(PathNode current)
        {
            PathNode node = new PathNode();
            // Position position = 
            return node;
        }

        private bool CanFitCuboid(Dector3 point1, Dector3 point2)
        {
            int fromX = Math.Min(point1.X, point2.X);
            int toX = Math.Max(point1.X, point2.X);

            int fromY = Math.Min(point1.Y, point2.Y);
            int toY = Math.Max(point1.Y, point2.Y);

            int fromZ = Math.Min(point1.Z, point2.Z);
            int toZ = Math.Max(point1.Z, point2.Z);

            foreach (var node in Nodes)
            {
                Dector3 d = node.Position;
                if (
                    (d.X > toX && d.X < fromX) ||
                    (d.Y > toY && d.Y < fromY) ||
                    (d.Z > toZ && d.Z < fromZ)
                )
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanPlaceNode(Dector3 d)
        {
            foreach (var node in Nodes)
            {
                if (node.Position.Equals(d))
                    return false;
            }

            return true;
        }

        /*
         * 2:51 17.05.2021 this would a scary and hungry shit
         * so im starting it
         */
        private Dector3[] GetFreeSpace(IEnumerable<Dector3> entries, Dector3 maxSize)
        {
            int[] extensions = {0, 0, 0, 0, 0, 0};
            while (true)
            {
                bool grew = false;
                foreach (var direction in Dector3.Directions)
                {
                    /*if (IsSpaceFree(entries, IntArrayToSize(extensions), direction))
                    {
                        grew = true;
                        extensions[Dector3.GetDirectionIndex(direction)] += 1;
                    }*/
                }

                if (!grew)
                    break;
            }

            /*
             * 3:20 17.05.2021
             * first version of this method is ready
             * looks a lot nicer than i expected
             * but i have to test it out
             *
             * never thought i'd care until now
             *
             * i hope they understand like i do
             */
            throw new NotImplementedException();
            //return new[] {entries};
        }

        private Dector3 IntArrayToSize(int[] array)
        {
            // represents a box
            Dector3 result = new Dector3(1, 1, 1);
            for (int i = 0; i < array.Length; i++)
            {
                result += Dector3.GetDirection(i) * array[i];
            }

            return result;
        }

        private bool IsSpaceFree(Dector3 entry, Dector3 currentSize, Dector3 direction)
        {
            Dector3 directionNonNormalized = Dector3.MultiplyCorrespondingAxis(currentSize, direction);
            Dector3 box = currentSize - directionNonNormalized + direction;
            Dector3 halfExtend = new Dector3((box.X - 1) / 2, (box.Y - 1) / 2, (box.Z - 1) / 2);
            Dector3 from = entry - halfExtend;
            Dector3 to = entry + halfExtend;
            return CanFitCuboid(from, to);
        }

        private void Reset()
        {
            _random = new Random(Config.Seed);
            Nodes.Clear();
            _currentPathLength = 0;
            _currentCanSpawnOffset = Config.MinimumOffsetBetweenMustSpawnPrototypes;
            _currentMustSpawnOffset = Config.MinimumOffsetBetweenCanSpawnPrototypes;
        }
    }
}