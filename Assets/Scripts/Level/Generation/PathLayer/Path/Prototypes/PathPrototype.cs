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
            if (GetSurfaceTilesCount() < this.entries.Count)
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


        private int GetSurfaceTilesCount()
        {
            int fullVolume = size.x * size.y * size.z;
            int innerVolume = (size.x - 2) * (size.y - 2) * (size.z - 2);

            return fullVolume - innerVolume;
        }
    }
}