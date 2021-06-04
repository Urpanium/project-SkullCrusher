using System;
using Level.Generation.PathLayer.Path.Map;
using Level.Generation.PathLayer.Path.Prototypes;
using Level.Generation.PathLayer.Path.Structures;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.SubGenerators
{
    /*
     * well, actually it's not generator
     */
    public class PrototypeGenerator : SubGenerator
    {
        public PrototypeGenerator(PathGenerationConfig config) : base(config)
        {
        }

        public bool TryFitPrototype(PathMap map, PathPrototype prototype, Dector3 entry, out Dector3 minPoint,
            out int rotation)
        {
            Dector3 entryDirection = GetEntryDirection(map, entry);
            Dector3 touchPoint = entry + entryDirection;

            foreach (Dector3 prototypeEntry in prototype.entries)
            {
                Dector3 prototypeMinPoint = touchPoint - prototypeEntry;
                for (int r = 0; r < 4; r++)
                {
                    Dector3 prototypeRotatedSize = prototype.size.Rotated(r);
                    if (CanFitCuboid(map, Cuboid.FromPosition(prototypeMinPoint, prototypeRotatedSize)))
                    {
                        rotation = r;
                        minPoint = prototypeMinPoint;
                        return true;
                    }
                }
            }

            rotation = -1;
            minPoint = new Dector3();
            return false;
        }
    }
}