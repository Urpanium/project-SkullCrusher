using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Level.Generation.PathLayer.Path.Decisions;
using Level.Generation.PathLayer.Path.Map;
using Level.Generation.PathLayer.Path.Prototypes;
using Level.Generation.PathLayer.Path.SubGenerators;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.Snapshots
{
    public class PathSnapshot
    {
        public PathMap map;
        public PathDecision decision;

        public Random random;
        public WeightedRandom weightedRandom;

        public int currentPathLength;
        
        public CorridorGenerator corridorGenerator;
        public RoomGenerator roomGenerator;
        public PrototypeGenerator prototypeGenerator;

        public int mustSpawnPrototypesRemain; 
        public int currentMustSpawnOffset;

        public int canSpawnPrototypesRemain;
        public int currentCanSpawnOffset;

        public List<Dector3> allEntries;
        public List<Dector3> currentEntries;
        
        public int restores;

        public PathSnapshot(PathMap map, PathDecision decision)
        {
            this.map = map;
            this.decision = decision;
            restores = 0;
        }

        private PathSnapshot()
        {
            
        }
        public static PathSnapshot Start(PathGenerationConfig config, Dector3 mapSize, int mustSpawnPrototypesCount, int canSpawnPrototypesCount)
        {
            PathSnapshot snapshot = new PathSnapshot
            {
                map = new PathMap(mapSize),
                decision = null,
                random = new Random(),
                weightedRandom = new WeightedRandom(config.seed),
                currentPathLength = 0,
                corridorGenerator = new CorridorGenerator(config),
                roomGenerator = new RoomGenerator(config),
                prototypeGenerator = new PrototypeGenerator(config),
                mustSpawnPrototypesRemain = mustSpawnPrototypesCount,
                currentMustSpawnOffset = 0,
                canSpawnPrototypesRemain = canSpawnPrototypesCount,
                currentCanSpawnOffset = 0,
                allEntries = new List<Dector3>(),
                currentEntries = new List<Dector3>(),
                restores = 0
            };
             
            return snapshot;
        }
    }
}