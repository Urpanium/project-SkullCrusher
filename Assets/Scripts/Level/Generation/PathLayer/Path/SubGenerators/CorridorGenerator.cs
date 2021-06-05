using System;
using System.Collections.Generic;
using System.Linq;
using Level.Generation.PathLayer.Path.Map;
using Level.Generation.PathLayer.Path.Structures;
using Level.Generation.Util;
using Random = System.Random;

namespace Level.Generation.PathLayer.Path.SubGenerators
{
    public class CorridorGenerator : SubGenerator
    {
        /*
         * protected PathGeneratorConfig config;
         * protected Random random;
         * protected WeightedRandom weightedRandom;
         */
        public CorridorGenerator(PathGenerationConfig config) : base(config)
        {
            this.config = config;
            random = new Random(config.seed);
        }


        public Cuboid Generate(PathMap map, Dector3 entry, out Dector3 newEntry)
        {
            Dector3 entryDirection = GetEntryDirection(map, entry);


            Dector3 startPosition = entry + entryDirection;

            List<Dector3> directions = Dector3.Directions.ToList();
            List<float> weights = config.PathDirectionsWeights.ToList();

            List<Dector3> directionsList = weightedRandom.RandomWeightedList(directions, weights);

            for (int i = 0; i < directionsList.Count; i++)
            {
                Dector3 direction = directionsList[i];
                UnityEngine.Debug.Log($"Is valid ({direction}: {IsValidEntryDirection(map, startPosition, direction)}");
                if (IsValidEntryDirection(map, startPosition, direction))
                {
                    Dector3 maxSize = GetMaxCorridorSize(map, startPosition, direction);
                    UnityEngine.Debug.Log($"Corridor max size: {maxSize}");
                    Dector3 minSize = new Dector3(1, 1, config.MinimumCorridorsLengths[i]);
                    if (maxSize.z >= minSize.z)
                    {
                        Dector3 size = Dector3.Random(random, minSize, maxSize);
                        if (size.x % 2 == 0)
                            size.x--;
                        if (size.y % 2 == 0)
                            size.y--;

                        PathTile tile = PathTile.FromByte(map.GetTile(entry));
                        tile.SetDirectionAccess(Dector3.GetDirectionIndex(entryDirection), true);

                        newEntry = entry + entryDirection * size.z;
                        UnityEngine.Debug.Log(
                            $"Generated corridor from {entry} to {newEntry}, width: {size.x}, height: {size.y}");
                        Cuboid cuboid = CorridorToCuboid(entry, newEntry, size.x, size.y);
                        UnityEngine.Debug.Log($"Corridor cuboid: {cuboid}");
                        return cuboid;
                    }
                }
            }
            /*
             * out of variants
             * can't build any corridor here
             */

            newEntry = new Dector3(-1, -1, -1);
            return Cuboid.FromPosition(Dector3.Zero, new Dector3(-1, -1, -1));
        }

        private Dector3 GetMaxCorridorSize(PathMap map, Dector3 entry, Dector3 entryDirection)
        {
            int entryDirectionIndex = Dector3.GetDirectionIndex(entryDirection);
            if (entryDirectionIndex == -1)
            {
                throw new Exception($"Corridor max size error: entry direction {entryDirection} index is -1");
            }

            Dector3 minPossibleSize = new Dector3(1, 1, config.MinimumCorridorsLengths[entryDirectionIndex]);
            Dector3 maxPossibleSize = new Dector3(
                config.MaximumCorridorsWidths[entryDirectionIndex],
                config.MaximumCorridorsHeights[entryDirectionIndex],
                config.MaximumCorridorsLengths[entryDirectionIndex]
            );

            UnityEngine.Debug.Log($"MAX LENGTH: {maxPossibleSize.z}");
            Dector3 size = new Dector3();
            /*
             * length is in priority
             * width and height can be swapped
             */

            for (int width = minPossibleSize.x; width <= maxPossibleSize.x; width++)
            {
                for (int height = minPossibleSize.y; height <= maxPossibleSize.y; height++)
                {
                    for (int length = minPossibleSize.z; length <= maxPossibleSize.z; length++)
                    {
                        if (CanFitCorridor(map, entry, entry + entryDirection * length, width, height))
                        {
                            size.x = width;
                            size.y = height;
                            size.z = length;
                        }
                        else
                        {
                            return size;
                        }
                    }
                }
            }


            return size;
        }


        private Cuboid CorridorToCuboid(Dector3 from, Dector3 to, int width, int height)
        {
            if (width % 2 == 0 || height % 2 == 0)
            {
                throw new Exception("Even width or/and height are not allowed");
            }
            UnityEngine.Debug.Log($"FROM BEFORE: {from}");
            UnityEngine.Debug.Log($"TO BEFORE: {to}");

            (Dector3, Dector3) minAndMax = Dector3.ToMinAndMax(from, to);
            from = minAndMax.Item1;
            to = minAndMax.Item2;
            UnityEngine.Debug.Log($"FROM AFTER: {from}");
            UnityEngine.Debug.Log($"TO AFTER: {to}");

            Dector3 direction = (to - from).ToOne();

            Dector3 up;
            Dector3 right;

            int halfWidth = width == 1 ? 0 : width / 2;
            int halfHeight = height == 1 ? 0 : height / 2;
            
            int directionIndex = Dector3.GetDirectionIndex(direction);
            
            if (!Dector3.IsDifferentOnlyByOneAxis(to, from))
            {
                throw new Exception($"Direction does not differs only by one axis: {direction}, from {from} to {to}, delta {to - from}");
            }
            
            if (directionIndex == -1)
            {
                throw new Exception(
                    $"Invalid direction: {direction}, from {from} to {to}, delta {(to - from)}, equals back: {direction.Equals(Dector3.Back)}");
            }

            /*
             * if up
             */
            if (directionIndex == 0 || directionIndex == 1) 
            {
                up = Dector3.Forward;
                right = Dector3.Right;
            }
            else
            {
                up = Dector3.Up;
                right = Dector3.GetDirection(2 + (directionIndex - 2 + 1) % 4);
            }

            
            Dector3 cuboidFrom = from 
                             + up * halfHeight
                             + right * halfWidth;
            Dector3 cuboidTo = to
                             - up * halfHeight
                             - right * halfWidth;
            UnityEngine.Debug.Log($"Cuboid: from = {from} + {up} * {halfHeight} + {right} * {halfWidth}; to {from} - {up} * {halfHeight} - {right} * {halfWidth}");
            return Cuboid.FromPoints(cuboidFrom, cuboidTo);
        }

        private bool CanFitCorridor(PathMap map, Dector3 from, Dector3 to, int width, int height)
        {
            return CanFitCuboid(map, CorridorToCuboid(from, to, width, height));
        }
    }
}