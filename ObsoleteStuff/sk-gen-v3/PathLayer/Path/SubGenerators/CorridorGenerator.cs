using System;
using System.Collections.Generic;
using Level.Generation.PathLayer.Path.Structures;
using Level.Generation.Util;
using UnityEngine;
using Random = System.Random;

namespace Level.Generation.PathLayer.Path.SubGenerators
{
    public class CorridorGenerator : SubGenerator
    {
        /*
         * protected PathGeneratorConfig config;
         * protected Random random;
         */
        public CorridorGenerator(PathGeneratorConfig config) : base(config)
        {
            this.config = config;
            random = new Random(config.seed);
        }


        public Cuboid GenerateCorridor(PathMap map, Dector3 entry, out Dector3 to, int decisionsCount = 0)
        {
            Dector3 entryDirection = GetEntryDirection(map, entry, decisionsCount);


            Dector3 startPosition = entry + entryDirection;

            List<Dector3> directionsList = GetRandomizedDirections(map, startPosition);

            for (int i = 0; i < Dector3.Directions().Length; i++)
            {
                /*
                 * TODO: maybe some shit
                 */
                Dector3 direction = directionsList[(i + decisionsCount) % directionsList.Count];
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

                        to = entry + entryDirection * size.z;
                        return CorridorToCuboid(entry, entry + entryDirection * size.z, size.x, size.y);
                    }
                }
            }
            /*
             * out of variants
             * can't build any corridor here
             */

            to = new Dector3(-1, -1, -1);
            return Cuboid.FromPosition(Dector3.Zero, new Dector3(-1, -1, -1));


            /*int startPositionDirectionIndex = Dector3.GetDirectionIndex(startPositionDirection);
            
            Dector3 maxSize = GetMaxCorridorSize(map, startPosition);
            Dector3 minSize = new Dector3(1, 1, config.MinimumCorridorsLengths[startPositionDirectionIndex]);

            if (maxSize.z < minSize.z)
            {
                UnityEngine.Debug.Log("minSize is greater than maxSize");
                to = entry;
                return Cuboid.FromPoints(entry, entry);
            }
            Dector3 size = Dector3.Random(random, minSize, maxSize);
            
            if (size.x % 2 == 0)
                size.x--;
            if (size.y % 2 == 0)
                size.y--;
            
            to = entry + entryDirection * size.z;
            return CorridorToCuboid(entry, entry + entryDirection * size.z, size.x, size.y);*/
        }

        private List<Dector3> GetRandomizedDirections(PathMap map, Dector3 startPosition)
        {
            /*
             * TODO: possible incorrect calculations
             */
            List<Dector3> result = new List<Dector3>();
            List<int> validDirectionsIndexes = new List<int>();
            for (int i = 0; i < Dector3.Directions().Length; i++)
            {
                Dector3 direction = Dector3.GetDirection(i);
                if (IsValidEntryDirection(map, startPosition, direction))
                {
                    validDirectionsIndexes.Add(i);
                }
            }

            for (int i = 0; i < validDirectionsIndexes.Count; i++)
            {
                float totalWeight = 0;
                for (int j = 0; j < validDirectionsIndexes.Count; j++)
                {
                    int index = validDirectionsIndexes[j];
                    totalWeight += config.PathDirectionsWeights[index];
                }

                int transferIndex = 0;
                float randomValue = (float) random.NextDouble() * totalWeight;
                for (int j = 0; j < validDirectionsIndexes.Count; j++)
                {
                    int index = validDirectionsIndexes[j];
                    randomValue -= config.PathDirectionsWeights[index];
                    if (randomValue <= 0)
                    {
                        transferIndex = index;
                        break;
                    }
                }

                if (!Dector3.GetDirection(transferIndex).Equals(Dector3.Zero))
                    result.Add(Dector3.GetDirection(transferIndex));

                validDirectionsIndexes.Remove(transferIndex);
            }


            UnityEngine.Debug.Log("WEIGHTED DIRECTIONS LIST:");
            foreach (var direction in result)
            {
                UnityEngine.Debug.Log($"Direction {direction}");
            }

            return result;
        }


        private Dector3 GetMaxCorridorSize(PathMap map, Dector3 entry, Dector3 entryDirection)
        {
            int entryDirectionIndex = Dector3.GetDirectionIndex(entryDirection);
            if (entryDirectionIndex == -1)
            {
                throw new Exception($"Corridor max size error: entry direction {entryDirection} index is -1");
            }

            /*UnityEngine.Debug.Log($"EntryDirection: {entryDirection}, index: {entryDirectionIndex}");*/
            Dector3 minPossibleSize = new Dector3(1, 1, config.MinimumCorridorsLengths[entryDirectionIndex]);
            Dector3 maxPossibleSize = new Dector3(
                config.MaximumCorridorsWidths[entryDirectionIndex],
                config.MaximumCorridorsHeights[entryDirectionIndex],
                config.MaximumCorridorsLengths[entryDirectionIndex]
            );

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
            if (width == 1)
                width = 0;
            else
                width /= 2;

            if (height == 1)
                height = 0;
            else
                height /= 2;


            Dector3 direction = (to - from).ToOne();

            Dector3 up;
            Dector3 right;

            int directionIndex = Dector3.GetDirectionIndex(direction);
            /*if (!Dector3.IsDifferentOnlyByOneAxis(to, from))
            {
                throw new Exception($"Direction does not differs only by one axis: {direction}, from {from} to {to}, delta {to - from}");
            }*/

            // какого хуя direction.Equals(Dector3.Back) даёт true, и при этом индекс D(0, 0, -1) равен -1 блять, что за говно?
            if (directionIndex == -1)
            {
                // ебать, direction равен (0, 0, -1), (to - from).ToOne равен (1, 1, 1), пиздец
                // пофиксил, все равно похуй ему на Dector3.Back в массиве Directions

                // пиздец
                throw new Exception(
                    $"Invalid direction: {direction} (got {directionIndex} index), from {from} to {to}, delta {(to - from)}, equals back: {direction.Equals(Dector3.Back)}");
            }

            if (directionIndex == 0)
            {
                up = Dector3.Forward;
                right = Dector3.Right;
            }
            else
            {
                up = Dector3.Up;
                right = Dector3.GetDirection(2 + (directionIndex - 2 + 1) % 4);
            }

            Dector3 point1 = from + up * height + right * width;
            Dector3 point2 = to - up * height - right * width;
            return Cuboid.FromPoints(point1, point2);
        }

        private bool CanFitCorridor(PathMap map, Dector3 from, Dector3 to, int width, int height)
        {
            return CanFitCuboid(map, CorridorToCuboid(from, to, width, height));
        }
    }
}