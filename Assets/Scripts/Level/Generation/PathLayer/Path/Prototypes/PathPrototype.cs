using System;
using System.Collections.Generic;
using System.Linq;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.Prototypes
{
    public class PathPrototype
    {
        public Dector3 size;
        public List<Dector3> entries;

        public PathPrototype(Dector3 size, IEnumerable<Dector3> entries)
        {
            this.size = size;
            this.entries = entries.ToList();
            if (GetSurfaceSquare() < this.entries.Count)
            {
                throw new Exception($"Too many entries received: {this.entries.Count}");
            }
        }
        
        
        private int GetSurfaceSquare()
        {
            int xFaces = size.z * size.y * 2;
            int yFaces = size.x * size.z * 2;
            int zFaces = size.y * size.x * 2;

            return xFaces + yFaces + zFaces;
        }
    }
}