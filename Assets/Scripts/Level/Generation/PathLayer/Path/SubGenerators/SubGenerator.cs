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

        protected Dector3 GetEntryDirection(PathMap map, Dector3 entry, int tryNumber = 0)
        {
            Random directionRandom = new Random(entry.x + entry.y * 10 + entry.z * 100);
            int randomOffset = directionRandom.Next(Dector3.Directions.Length);

            for (int i = 0; i < Dector3.Directions.Length; i++)
            {
                int directionIndex = (i + tryNumber + randomOffset) % Dector3.Directions.Length;
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

        protected Cuboid GetFreeSpace(PathMap map, Dector3 entry, bool sameFloor, Dector3 maxSize)
        {
            Dector3 minBound = entry;
            Dector3 maxBound = entry;

            Dector3 size = new Dector3();
            while (size.x < maxSize.x || size.y < maxSize.y || size.z < maxSize.z)
            {
                bool extended = false;

                for (int i = 0; i < Dector3.Directions.Length; i++)
                {
                    Dector3 direction = Dector3.Directions[i];

                    bool isPositive = direction.Equals(
                        new Dector3(
                            Math.Abs(direction.x),
                        Math.Abs(direction.y),
                        Math.Abs(direction.z)
                        ));

                    Dector3 tryNewMinBound = minBound;
                    Dector3 tryNewMaxBound = maxBound;

                    if (sameFloor && !isPositive)
                    {
                        direction.y = 0;
                    }

                    if (isPositive)
                    {
                        tryNewMaxBound += direction;
                    }
                    else
                    {
                        tryNewMinBound += direction;
                    }

                    Cuboid cuboid = Cuboid.FromPoints(tryNewMinBound, tryNewMaxBound);
                    UnityEngine.Debug.Log($"Room cuboid {cuboid}");
                    if (CanFitCuboid(map, cuboid))
                    {
                        UnityEngine.Debug.Log($"Can fit");
                        if (sameFloor)
                        {
                            minBound.x = tryNewMinBound.x;
                            minBound.z = tryNewMinBound.y;
                        }
                        else
                        {
                            minBound = tryNewMinBound;
                        }

                        maxBound = tryNewMaxBound;
                        extended = true;
                    }
                }


                if (!extended)
                    break;
                size = maxBound - minBound;
            }

            if (maxBound.x > maxSize.x - minBound.x)
            {
                maxBound.x = minBound.x + maxSize.x;
            }
            
            if (maxBound.y > maxSize.y - minBound.y)
            {
                maxBound.y = minBound.y + maxSize.y;
            }

            if (maxBound.z > maxSize.z - minBound.z)
            {
                maxBound.z = minBound.z + maxSize.z;
            }

            //throw new NotImplementedException();
            return Cuboid.FromPoints(minBound, maxBound);
        }

        /*protected Cuboid GetFreeSpace(PathMap map, Dector3 entry, Dector3 maxSize, int differ = 0)
        {
            Dector3 minBound = entry;
            Dector3 maxBound = entry;

            /*
             * length is in priority 
             * width and height can be swapped
             #1#
            Dector3 size = maxBound - minBound;
            while (size.x < maxSize.x && size.y < maxSize.y && size.z < maxSize.z)
            {
                bool extended = false;

                /*
                 * TODO: expand cuboid
                 #1#
                Dector3[] points =
                {
                    minBound, // i i i
                    maxBound, // a a a

                    new Dector3(minBound.x, minBound.y, maxBound.z), // i i a
                    new Dector3(minBound.x, maxBound.y, minBound.z), // i a i
                    new Dector3(maxBound.x, minBound.y, minBound.z), // a i i

                    new Dector3(minBound.x, maxBound.y, maxBound.z), // i a a
                    new Dector3(maxBound.x, minBound.y, maxBound.z), // a i a
                    new Dector3(maxBound.x, maxBound.y, minBound.z) // a a i
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
        }*/


        private byte IsNewBound(Dector3 minBound, Dector3 maxBound, Dector3 point)
        {
            if (
                point.x < minBound.x ||
                point.y < minBound.y ||
                point.z < minBound.z
            )
            {
                return 1;
            }

            if (
                point.x > maxBound.x ||
                point.y > maxBound.y ||
                point.z > maxBound.z
            )
            {
                // долбоёб блять
                return 2;
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