using System;
using Level.Generation.PathLayer.Path.Structures;
using Level.Generation.Util;

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


        public Cuboid GenerateCorridor(PathMap map, Dector3 entry, out long variants)
        {
            Dector3 entryDirection = GetEntryDirection(map, entry);
            int entryDirectionIndex = Dector3.GetDirectionIndex(entryDirection);
            
            Dector3 maxSize = GetMaxCorridorSize(map, entry, out variants);
            Dector3 minSize = new Dector3(1, 1, config.MinimumCorridorsLengths[entryDirectionIndex]);

            Dector3 size = Dector3.Random(random, minSize, maxSize);
            return CorridorToCuboid(entry, entry + entryDirection * size.z, size.x, size.y);
        }

        private Dector3 GetMaxCorridorSize(PathMap map, Dector3 entry, out long variants)
        {
            Dector3 entryDirection = GetEntryDirection(map, entry);
            int entryDirectionIndex = Dector3.GetDirectionIndex(entryDirection);

            Dector3 minPossibleSize = new Dector3(1, 1, config.MinimumCorridorsLengths[entryDirectionIndex]);
            Dector3 maxPossibleSize = new Dector3(
                config.MaximumCorridorsWidths[entryDirectionIndex],
                config.MaximumCorridorsHeights[entryDirectionIndex],
                config.MaximumCorridorsLengths[entryDirectionIndex]
            );

            Dector3 size = new Dector3();
            variants = 0;
            /*
             * length is in priority
             * width and height can be swapped
             */
            for (short length = minPossibleSize.z; length < maxPossibleSize.z; length++)
            {
                for (short width = minPossibleSize.x; width < maxPossibleSize.x; width++)
                {
                    for (short height = minPossibleSize.y; height < maxPossibleSize.y; height++)
                    {
                        if (CanFitCorridor(map, entry, entry + entryDirection * length, width, height))
                        {
                            size.x = width;
                            size.y = height;
                            size.z = length;
                            variants++;
                        }
                    }
                }
            }

            if (size.x % 2 == 0)
                size.x--;
            if (size.y % 2 == 0)
                size.y--;
            return size;
        }


        private Cuboid CorridorToCuboid(Dector3 from, Dector3 to, int width, int height)
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
            return Cuboid.FromPoints(point1, point2);
        }

        private bool CanFitCorridor(PathMap map, Dector3 from, Dector3 to, int width, int height)
        {
            return CanFitCuboid(map, CorridorToCuboid(from, to, width, height));
        }
    }
}