using System;
using System.Collections.Generic;
using Level.Gen.Util;
using Level.Gen.VisualLayer;
using Level.Generation.PathLayer.PathGeneration;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Snapshots
{
    public class PathGenerationSnapshot
    {
        public Queue<Prototype> CurrentMustSpawnPrototypesQueue { get; set; }
        
        public List<Dector3> CurrentEntries { get; set; }
        public List<Dector3> OpenEntries { get; set; }
        public Map3<PathNode> Map { get; set; }
        public int CurrentPathLength { get; set; }

        public int CurrentMustSpawnOffset { get; set; }
        public int CurrentCanSpawnOffset { get; set; }

        public Random Random { get; set; }

        public int RestoresCount { get; set; }

        public PathGenerationSnapshot()
        {
            RestoresCount = 0;
        }


        public PathGenerationSnapshot(Queue<Prototype> currentMustSpawnPrototypesQueue, List<Dector3> openEntries,
            Map3<PathNode> map, int currentPathLength, int currentCanSpawnOffset, int currentMustSpawnOffset,
            Random random)
        {
            CurrentMustSpawnPrototypesQueue = currentMustSpawnPrototypesQueue;
            OpenEntries = openEntries;
            Map = map;
            CurrentPathLength = currentPathLength;
            CurrentCanSpawnOffset = currentCanSpawnOffset;
            CurrentMustSpawnOffset = currentMustSpawnOffset;
            Random = random;
            RestoresCount = 0;
        }
    }
}