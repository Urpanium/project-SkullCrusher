using System;
using System.Collections.Generic;
using System.Linq;
using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.Prototypes
{
    public class PathPrototype
    {
        public Dector3 size;

        public int rotation;

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

        public List<Dector3> GetRotatedEntries()
        {
            Dector3 center = size / 2;
            List<Dector3> result = entries;
            for (int i = 0; i < entries.Count; i++)
            {
                result[i] = center + (result[i] - center).Rotated(rotation);
            }

            return result;
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