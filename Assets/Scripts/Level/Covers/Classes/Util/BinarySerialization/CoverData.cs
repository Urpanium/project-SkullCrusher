using System;

namespace Level.Covers.Classes.Util.BinarySerialization
{
    [Serializable]
    public class CoverData
    {
        public Vector3Data position;
        public Vector3Data direction;
        public CoverType type;
        
        public CoverData(Cover cover)
        {
            position = new Vector3Data(cover.position);
            direction = new Vector3Data(cover.direction);
            type = cover.type;
        }

        public Cover ToCover()
        {
            Cover result = new Cover(position.ToVector3(), direction.ToVector3(), type);
            return result;
        }
    }
}