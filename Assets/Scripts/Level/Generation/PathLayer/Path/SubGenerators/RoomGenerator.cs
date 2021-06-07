using System;
using System.Collections.Generic;
using Level.Generation.PathLayer.Path.Map;
using Level.Generation.PathLayer.Path.Structures;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.SubGenerators
{
    public class RoomGenerator: SubGenerator
    {
        public RoomGenerator(PathGenerationConfig config) : base(config)
        {
        }

        public Cuboid Generate(PathMap map, Dector3 entry, out List<Dector3> newEntries)
        {
            Cuboid freeSpace = GetFreeSpace(map, entry, config.maximumRoomSize);

            Dector3 size = Dector3.Random(random, config.minimumRoomSize, freeSpace.size);
            
            /*
             * TODO: generate entries
             */
            Cuboid roomCuboid = Cuboid.FromPosition(freeSpace.position, size);
            int entriesCount = random.Next(config.minimumRoomEntriesCount,
                Math.Min(roomCuboid.GetSurfaceTilesCount(), config.maximumRoomEntriesCount));

            newEntries = roomCuboid.GenerateRoomEntries(random, entriesCount);
            return roomCuboid;
        }
    }
}