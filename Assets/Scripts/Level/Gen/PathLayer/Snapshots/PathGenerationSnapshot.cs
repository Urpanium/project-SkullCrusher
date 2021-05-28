using System;
using System.Collections.Generic;
using WaveFunctionCollapse3D.PathLayer.PathGeneration;
using WaveFunctionCollapse3D.Util;
using WaveFunctionCollapse3D.VisualLayer;

namespace WaveFunctionCollapse3D.PathLayer.Snapshots
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