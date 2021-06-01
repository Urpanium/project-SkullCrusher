using System;
using System.Collections.Generic;
using Level.Generation.PathLayer.Path.Prototypes;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path
{
    public class PathGenerator
    {
        public PathGeneratorConfig config;

        private List<PathPrototype> mustSpawnPrototypes;
        private List<PathPrototype> canSpawnPrototypes;

        private bool debugMode;

        public PathGenerator(
            PathGeneratorConfig config,
            List<PathPrototype> mustSpawnPrototypes,
            List<PathPrototype> canSpawnPrototypes
        )
        {
            this.config = config;
            this.mustSpawnPrototypes = mustSpawnPrototypes;
            this.canSpawnPrototypes = canSpawnPrototypes;
            debugMode = mustSpawnPrototypes.Count < 2;
        }

        public PathMap Generate()
        {
            Dector3 mapSize = GetMapSize();
            PathMap map = new PathMap(mapSize);

            /*
             * GENERATE SOMETHING
             */

            return map;
        }


        private Dector3 GetMapSize()
        {
            /*
             * remember: no LINQ
             */

            Dector3 prototypesSizes = new Dector3();
            foreach (var prototype in mustSpawnPrototypes)
            {
                prototypesSizes += prototype.size;
            }

            foreach (var prototype in canSpawnPrototypes)
            {
                prototypesSizes += prototype.size;
            }

            Dector3 corridorsSizes = new Dector3();
            int configPathLength = config.maximumPathLength;
            float[] pathDirectionsWeightsNormalized = config.PathDirectionsWeights;


            for (int i = 0; i < 6; i++)
            {
                float multiplier = configPathLength * config.MaximumCorridorsLengths[i] *
                                   pathDirectionsWeightsNormalized[i];
                corridorsSizes += Dector3.Directions[i] * (int) Math.Round(multiplier + 0.4f);
            }


            Dector3 mapSize = (corridorsSizes + prototypesSizes) * 2;
            
            return mapSize;
        }
    }
}