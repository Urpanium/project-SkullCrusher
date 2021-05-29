using System.Collections.Generic;
using System.Linq;
using Level.Gen.Util;

namespace Level.Gen.PathLayer.Path.PathPrototypes
{
    public class PathPrototype
    {
        public Dector3 size;
        public List<Dector3> socketsPositions;

        public PathPrototype(Dector3 size, IEnumerable<Dector3> socketsPositions)
        {
            this.size = size;
            this.socketsPositions = socketsPositions.ToList();
            if (GetSurfaceSquare() < this.socketsPositions.Count)
            {
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