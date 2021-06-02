﻿using System;
using Level.Generation.PathLayer.Path.Structures;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.SubGenerators
{
    public class SubGenerator
    {
        public PathGeneratorConfig config;

        protected Random random;

        public SubGenerator(PathGeneratorConfig config)
        {
            this.config = config;
            random = new Random(config.seed);
        }

        protected static Dector3 GetEntryDirection(PathMap map, Dector3 entry, int offset = 0)
        {
            Random directionRandom = new Random(entry.x + entry.y * 10 + entry.z * 100);
            int randomOffset = directionRandom.Next(Dector3.Directions.Length);
            
            for (int i = 0; i < Dector3.Directions.Length; i++)
            {
                int directionIndex = (i + offset + randomOffset) % Dector3.Directions.Length;
                Dector3 direction = Dector3.Directions[directionIndex];
                if (!map.IsTileEmpty(entry + direction) && !map.IsTileEmpty(entry - direction))
                {
                    return direction;
                }
            }

            throw new Exception($"Could not determine direction of entry at position {entry}");
            /*
             * my time will come soon
             * return Dector3.Directions[random.Next(Dector3.Directions.Length)];
             */
        }

        protected static bool CanFitCuboid(PathMap map, Cuboid cuboid)
        {
            Dector3 from = cuboid.position;
            Dector3 to = cuboid.To();

            map.ValidatePosition(from);
            map.ValidatePosition(to);

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