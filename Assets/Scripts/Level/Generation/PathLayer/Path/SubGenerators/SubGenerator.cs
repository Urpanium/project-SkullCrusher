using System;
using Level.Generation.PathLayer.Path.Map;
using Level.Generation.PathLayer.Path.Structures;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.SubGenerators
{
    public class SubGenerator
    {
        public PathGenerationConfig config;

        protected Random random;
        protected WeightedRandom weightedRandom;

        public SubGenerator(PathGenerationConfig config)
        {
            this.config = config;
            random = new Random(config.seed);
            weightedRandom = new WeightedRandom(config.seed);
        }

        protected Dector3 GetEntryDirection(PathMap map, Dector3 entry, int offset = 0)
        {
            Random directionRandom = new Random(entry.x + entry.y * 10 + entry.z * 100);
            int randomOffset = directionRandom.Next(Dector3.Directions.Length);

            for (int i = 0; i < Dector3.Directions.Length; i++)
            {
                int directionIndex = (i + offset + randomOffset) % Dector3.Directions.Length;
                Dector3 direction = Dector3.GetDirection(directionIndex);
                if (map.IsPositionValid(entry + direction)
                    /*&& map.IsPositionValid(entry - direction) */
                    && map.IsTileEmpty(entry + direction)
                    /*&& map.IsTileEmpty(entry - direction)*/)
                {
                    return direction.ToOne();
                }
            }

            /*
             * throw new Exception($"Could not determine direction of entry at position {entry} )");
             */

            return Dector3.GetDirection(random.Next(Dector3.Directions.Length));
        }

        protected Cuboid GetFreeSpace(PathMap map, Dector3 entry, Dector3 maxSize)
        {
            Dector3 entryDirection = GetEntryDirection(map, entry);
            int entryDirectionIndex = Dector3.GetDirectionIndex(entryDirection);
            if (entryDirectionIndex == -1)
            {
                throw new Exception($"Corridor max size error: entry direction {entryDirection} index is -1");
            }

            Dector3 minBound = new Dector3();
            Dector3 maxBound = new Dector3();

            /*
             * length is in priority 
             * width and height can be swapped
             */
            Dector3 size = maxBound - minBound;
            while (size.x < maxSize.x && size.y < maxSize.y && size.z < maxSize.z)
            {
                bool extended = false;

                /*
                 * TODO: expand cuboid
                 */
                Dector3[] points =
                {
                    minBound,
                    maxBound,

                    new Dector3(minBound.x, minBound.y, maxBound.z),
                    new Dector3(minBound.x, maxBound.y, minBound.z),
                    new Dector3(maxBound.x, minBound.y, minBound.z),

                    new Dector3(minBound.x, maxBound.y, maxBound.z),
                    new Dector3(maxBound.x, minBound.y, maxBound.z),
                    new Dector3(maxBound.x, maxBound.y, minBound.z)
                };

                for (int i = 0; i < points.Length; i++)
                {
                    Dector3 point = points[i];
                    for (int j = 0; j < Dector3.Directions.Length; j++)
                    {
                        Dector3 direction = Dector3.Directions[j];
                        Dector3 newPoint = point + direction;
                        
                        byte boundResult = IsNewBound(minBound, maxBound, newPoint);
                        
                        if (boundResult != 0)
                        {
                            Dector3 newMinBound = minBound;
                            Dector3 newMaxBound = maxBound;
                            if (boundResult == 1)
                                newMinBound = newPoint;
                            else
                                newMaxBound = newPoint;

                            Cuboid expandedCuboid = Cuboid.FromPoints(newMinBound, newMaxBound);
                            if (CanFitCuboid(map, expandedCuboid))
                            {
                                minBound = newMinBound;
                                maxBound = newMaxBound;
                                extended = true;
                            }
                        }
                    }
                }

                if (!extended)
                    break;
                size = maxBound - minBound;
            }


            return Cuboid.FromPoints(minBound, maxBound);
        }


        private byte IsNewBound(Dector3 minBound, Dector3 maxBound, Dector3 point)
        {
            if (
                point.x < minBound.x &&
                point.y < minBound.y &&
                point.z < minBound.z
            )
            {
                return 1;
            }

            if (
                point.x > maxBound.x &&
                point.y > maxBound.y &&
                point.z > maxBound.z
            )
            {
                return 1;
            }


            return 0;
        }


        protected bool IsValidEntryDirection(PathMap map, Dector3 entry, Dector3 direction)
        {
            return map.IsPositionValid(entry + direction)
                   && map.IsTileEmpty(entry + direction);
        }


        protected static bool CanFitCuboid(PathMap map, Cuboid cuboid)
        {
            Dector3 from = cuboid.position;
            Dector3 to = cuboid.To();
            if (
                !map.IsPositionValid(from) ||
                !map.IsPositionValid(to))
            {
                //UnityEngine.Debug.Log($"Can't fit because of invalid point(s): {cuboid}");
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
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}