using Level.Generation.Util;

namespace Level.Generation.PathLayer.Path.Structures
{
    public class Cuboid
    {
        public Dector3 position;
        public Dector3 size;

        private Cuboid(Dector3 position, Dector3 size)
        {
            this.position = position;
            this.size = size;
        }

        public static Cuboid FromPoints(Dector3 point1, Dector3 point2)
        {
            (Dector3, Dector3) minAndMax = Dector3.ToMinAndMax(point1, point2);
            Dector3 position = minAndMax.Item1;
            Dector3 size = minAndMax.Item2 - minAndMax.Item1;
            return new Cuboid(position, size);
        }

        public static Cuboid FromPosition(Dector3 position, Dector3 size)
        {
            return new Cuboid(position, size);
        }
        public Dector3 To()
        {
            return position + size;
        }
    }
}