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

        public bool IsInside(Dector3 entry, bool strict = true)
        {
            Dector3 from = entry;
            Dector3 to = To();
            if (strict)
            {
                return entry.x > from.x && entry.x < to.x
                                           && entry.y > from.y && entry.y < to.y
                                           && entry.z > from.z && entry.z < to.z;
            }

            return entry.x >= from.x && entry.x <= to.x
                                        && entry.y >= from.y && entry.y <= to.y
                                        && entry.z >= from.z && entry.z <= to.z;
        }
        public bool IsOnBorder(Dector3 entry)
        {
            Dector3 from = position;
            Dector3 to = To();
            return
                entry.x == from.x
                || entry.y == from.y
                || entry.z == from.z
                || entry.x == to.x
                || entry.y == to.y
                || entry.z == to.z;
        }

        public Dector3 To()
        {
            return position + size;
        }

        public override string ToString()
        {
            Dector3 to = To();
            return $"({position.x} {position.y} {position.z}) to ({to.x} {to.y} {to.z})";
        }
    }
}