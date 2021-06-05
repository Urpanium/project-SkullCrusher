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
            
            throw new NotImplementedException();
        }
    }
}