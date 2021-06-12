using System;
using System.Collections.Generic;
using Level.Generation.Util;
using NUnit.Framework;

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
            Dector3 from = position;
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

        private bool IsOnBorder(Dector3 entry)
        {
            return
                entry.x == 0
                || entry.y == 0
                || entry.z == 0
                || entry.x == size.x
                || entry.y == size.y
                || entry.z == size.z;
            /*Dector3 from = position + new Dector3(1, 1, 1);
            Dector3 to = To() + new Dector3(-1, -1, -1);
            return
                entry.x == from.x
                || entry.y == from.y
                || entry.z == from.z
                || entry.x == to.x
                || entry.y == to.y
                || entry.z == to.z;*/
        }

        public List<Dector3> GenerateRoomEntries(Random random, int count)
        {
            List<Dector3> allEntries = new List<Dector3>();
            List<Dector3> result = new List<Dector3>();
            int maxCount = GetSurfaceTilesCount();
            if (count > maxCount)
            {
                throw new Exception("Entries count is too big");
            }


            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        Dector3 point = new Dector3(x, y, z);
                        if (IsOnBorder(point))
                        {
                            allEntries.Add(position + point);
                        }
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                int index = random.Next(allEntries.Count);
                UnityEngine.Debug.Log($"Index {index} (collection size: {allEntries.Count})");
                result.Add(allEntries[index]);
                allEntries.RemoveAt(index);
            }

            return result;
        }

        public int GetSurfaceTilesCount()
        {
            // wooow, so optimized, my god
            int fullVolume = size.x * size.y * size.z;
            int innerVolume = (size.x - 2) * (size.y - 2) * (size.z - 2);

            return fullVolume - innerVolume;
        }

        public Dector3 To()
        {
            return position + size;
        }

        public override string ToString()
        {
            Dector3 to = To();
            return $"C ({position.x} {position.y} {position.z}) to ({to.x} {to.y} {to.z})";
        }
    }
}