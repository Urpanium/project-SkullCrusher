using System;
using System.Collections.Generic;
using Level.Generation.PathLayer.Path.Map;
using Level.Generation.PathLayer.Path.Structures;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.SubGenerators
{
    public class RoomGenerator : SubGenerator
    {
        public RoomGenerator(PathGenerationConfig config) : base(config)
        {
        }

        public Cuboid Generate(PathMap map, Dector3 entry, out List<Dector3> newEntries, bool sameFloor, int tryNumber)
        {
            random = new Random(config.seed + tryNumber);
            Dector3 entryDirection = GetEntryDirection(map, entry);
            if (sameFloor && entryDirection.Equals(Dector3.Down))
            {
                newEntries = null;
                return Cuboid.FromPosition(new Dector3(-1, -1, -1), new Dector3(-1, -1, -1));
            }
            Dector3 startPoint = entry + entryDirection;
                
            Cuboid freeSpace = GetFreeSpace(map, startPoint, sameFloor, config.maximumRoomSize);
            UnityEngine.Debug.Log($"Room free space: {freeSpace}");
            
            Dector3 m = config.minimumRoomSize;
            Dector3 f = freeSpace.size;
            
            if (m.x > f.x || m.y > f.y || m.z > f.z)
            {
                newEntries = null;
                return Cuboid.FromPosition(new Dector3(-1, -1, -1), new Dector3(-1, -1, -1));
            }

            Dector3 size = Dector3.Random(random, config.minimumRoomSize, freeSpace.size);

            Cuboid roomCuboid = Cuboid.FromPosition(freeSpace.position, size);
            int entriesCount = random.Next(config.minimumRoomEntriesCount,
                Math.Min(roomCuboid.GetSurfaceTilesCount(), config.maximumRoomEntriesCount));

            newEntries = roomCuboid.GenerateRoomEntries(random, entriesCount);
            return roomCuboid;
        }
    }
}