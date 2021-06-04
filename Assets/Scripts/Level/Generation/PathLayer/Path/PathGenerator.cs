using System;
using System.Collections.Generic;
using Level.Generation.PathLayer.Path.Map;
using Level.Generation.PathLayer.Path.Prototypes;
using Level.Generation.PathLayer.Path.Snapshots;
using Level.Generation.PathLayer.Path.Structures;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path
{
    public class PathGenerator
    {
        public PathGenerationConfig config;

        private List<PathPrototype> mustSpawnPrototypes;
        private List<PathPrototype> canSpawnPrototypes;


        private List<PathSnapshot> snapshots;

        private PathSnapshot currentSnapshot;

        public PathGenerator(PathGenerationConfig config, List<PathPrototype> mustSpawnPrototypes,
            List<PathPrototype> canSpawnPrototypes)
        {
            if (mustSpawnPrototypes == null)
                mustSpawnPrototypes = new List<PathPrototype>();
            if (canSpawnPrototypes == null)
                canSpawnPrototypes = new List<PathPrototype>();
            this.mustSpawnPrototypes = mustSpawnPrototypes;
            this.canSpawnPrototypes = canSpawnPrototypes;
            this.config = config;

            currentSnapshot =
                PathSnapshot.Start(config, GetMapSize(), mustSpawnPrototypes.Count, canSpawnPrototypes.Count);
        }

        public PathMap Generate()
        {
            Dector3 center = currentSnapshot.map.size / 2;
            BuildCuboid(Cuboid.FromPoints(center + new Dector3(-2, -2, -2), center/* + new Dector3(1, 1, 1)*/),
                new List<Dector3>());
            Dector3.DirCheck();
            return currentSnapshot.map;
        }

        public void Reset()
        {
            if (snapshots == null)
                snapshots = new List<PathSnapshot>();
            snapshots.Clear();
            currentSnapshot =
                PathSnapshot.Start(config, GetMapSize(), mustSpawnPrototypes.Count, canSpawnPrototypes.Count);
        }

        public void BuildCuboid(Cuboid cuboid, List<Dector3> entries, bool checkPointsOfCuboidOnBorder = false,
            bool changeable = false)
        {
            PathMap map = currentSnapshot.map;

            Dector3 from = cuboid.position;
            Dector3 to = cuboid.To();
            //UnityEngine.Debug.Log($"FROM {from} to {to}");
            for (int x = from.x; x <= to.x; x++)
            {
                for (int y = from.y; y <= to.y; y++)
                {
                    for (int z = from.z; z <= to.z; z++)
                    {
                        Dector3 position = new Dector3(x, y, z);
                        
                        PathTile pathTile = new PathTile();
                        UnityEngine.Debug.Log($"Checking position {position}");
                        if (cuboid.IsInside(position) || entries.Contains(position))
                        {
                            UnityEngine.Debug.Log("Any dir allowed");
                            pathTile.AllowGoToAnyDirection();
                            pathTile.Changeable = changeable;
                        }
                        else
                        {
                            
                            for (int i = 0; i < Dector3.Directions.Length; i++)
                            {

                                Dector3 direction = Dector3.GetDirection(i);
                                Dector3 nearPosition = position + direction;
                                
                                UnityEngine.Debug.Log($"Direction {direction}, near position {nearPosition}");

                                bool isValid = map.IsPositionValid(nearPosition);
                                bool isInside = cuboid.IsInside(nearPosition, false);
                                
                                
                                bool access = isValid && isInside;
                                UnityEngine.Debug.Log($"isValid: {isValid}; isInside: {isInside}; , access: {access}");
                                
                                pathTile.SetDirectionAccess(Dector3.GetDirectionIndex(direction),
                                    access);
                            }

                            //UnityEngine.Debug.Log("SET");
                            
                        }
                        map.SetTile(position, pathTile);
                    }
                }
            }
        }

        public void BuildPrototype(PathPrototype prototype)
        {
            PathMap map = currentSnapshot.map;
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
            float maxDirectionWeight = 0.01f;
            foreach (var t in pathDirectionsWeightsNormalized)
            {
                if (t > maxDirectionWeight)
                    maxDirectionWeight = t;
            }

            for (int i = 0; i < pathDirectionsWeightsNormalized.Length; i++)
            {
                pathDirectionsWeightsNormalized[i] /= maxDirectionWeight;
            }


            for (int i = 0; i < 6; i++)
            {
                float multiplier = configPathLength * config.MaximumCorridorsLengths[i] *
                                   pathDirectionsWeightsNormalized[i];
                corridorsSizes += Dector3.Directions[i].WithAbsAxis() *
                                  (int) Math.Round(multiplier + 0.4f);
            }


            Dector3 mapSize = (corridorsSizes + prototypesSizes) *
                              (int) Math.Round(config.mapSizeSafetyMultiplier + 0.4f);

            UnityEngine.Debug.Log($"GENERATED MAP SIZE: {mapSize}");
            return mapSize;
        }
    }
}