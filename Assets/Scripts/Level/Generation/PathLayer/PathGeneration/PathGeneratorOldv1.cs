using System;
using System.Collections.Generic;
using System.Linq;
using Level.Gen.Util;
using Level.Gen.VisualLayer;
using Level.Generation.PathLayer.Path;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.PathGeneration
{
    /*
     * so this old version of path generator (iteration #1)
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
            _random = new Random(Config.seed);
        }


        public PathGeneratorOld(PathGeneratorConfig config)
        {
            Config = config;
            _random = new Random(Config.seed);
        }

        public void Generate()
        {
            Reset();

            // place start node
            PathNode startNode = new PathNode
            {
                Position =
                {
                    x = 0,
                    y = 0,
                    z = 0,
                }
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
                (int) Math.Floor((float) (Config.maximumPathLength - 2) / requiredPrototypesCount);

            /*
             * -2 for start and end nodes
             */
            int minLengthPerCorridor =
                (int) Math.Floor((float) (Config.minimumPathLength - 2) / requiredPrototypesCount);


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
                int width = _random.Next(Config.minimumRoomXSize, Config.maximumRoomXSize);
                int height = _random.Next(Config.minimumRoomYSize, Config.maximumRoomYSize);
                int length = _random.Next(Config.minimumRoomZSize, Config.maximumRoomZSize);

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

        private bool IsEntriesOnBorderOfCuboid(IEnumerable<Dector3> entries, Dector3 point1, Dector3 point2)
        {
            /*Dector3 point2 = point1 + cuboidSize;*/
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

        private bool HasAlternativePaths()
        {
            // check if checkpoints have alternative path which allow to skip important map fragment
            throw new NotImplementedException();
        }

        private void PlaceRoom(Dector3 where, Dector3 size, List<Dector3> entriesLocalPositions)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
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

                            if (x == size.x - 1)
                            {
                                roomNode.CanGoToRight = false;
                            }

                            if (y == 0)
                            {
                                roomNode.CanGoToBottom = false;
                            }

                            if (y == size.y - 1)
                            {
                                roomNode.CanGoToTop = false;
                            }

                            if (z == 0)
                            {
                                roomNode.CanGoToBack = false;
                            }

                            if (z == size.z - 1)
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
            int x1 = where.x;
            int x2 = where.x + size.x;

            int y1 = where.y;
            int y2 = where.y + size.y;

            int z1 = where.z;
            int z2 = where.z + size.z;

            foreach (var node in Nodes)
            {
                Dector3 p = node.Position;
                if (
                    p.x > x1 && p.x < x2 ||
                    p.y > y1 && p.y < y2 ||
                    p.z > z1 && p.z < z2
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
            int fromX = Math.Min(point1.x, point2.x);
            int toX = Math.Max(point1.x, point2.x);

            int fromY = Math.Min(point1.y, point2.y);
            int toY = Math.Max(point1.y, point2.y);

            int fromZ = Math.Min(point1.z, point2.z);
            int toZ = Math.Max(point1.z, point2.z);

            foreach (var node in Nodes)
            {
                Dector3 d = node.Position;
                if (
                    (d.x > toX && d.x < fromX) ||
                    (d.y > toY && d.y < fromY) ||
                    (d.z > toZ && d.z < fromZ)
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
            Dector3 halfExtend = new Dector3((box.x - 1) / 2, (box.y - 1) / 2, (box.z - 1) / 2);
            Dector3 from = entry - halfExtend;
            Dector3 to = entry + halfExtend;
            return CanFitCuboid(from, to);
        }

        private void Reset()
        {
            _random = new Random(Config.seed);
            Nodes.Clear();
            _currentPathLength = 0;
            _currentCanSpawnOffset = Config.minimumOffsetBetweenMustSpawnPrototypes;
            _currentMustSpawnOffset = Config.minimumOffsetBetweenCanSpawnPrototypes;
        }
    }
}