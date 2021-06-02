using System;
using System.Collections.Generic;
using Level.Generation.PathLayer.Path.Decisions;
using Level.Generation.PathLayer.Path.SubGenerators;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.Snapshots
{
    public class PathSnapshot
    {
        public PathMap map;
        public PathDecision decision;

        public Random random;
        
        public CorridorGenerator corridorGenerator;
        public RoomGenerator roomGenerator;
        public PrototypeGenerator prototypeGenerator;

        public int mustSpawnPrototypesRemain; 
        public int currentMustSpawnOffset;

        public int canSpawnPrototypesRemain;
        public int currentCanSpawnOffset;
        
        public List<Dector3> currentEntries;
        
        public int restores;

        public PathSnapshot(PathMap map, PathDecision decision)
        {
            this.map = map;
            this.decision = decision;
            restores = 0;
        }
    }
}