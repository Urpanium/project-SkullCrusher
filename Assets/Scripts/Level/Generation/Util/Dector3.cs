using System;
using UnityEngine;
using Random = System.Random;

namespace Level.Generation.Util
{
    [Serializable]
    public class Dector3
    {
        // Discrete Vector3

        public int x;
        public int y;
        public int z;


        public const int UpIndex = 0;
        public const int DownIndex = 1;
        public const int ForwardIndex = 2;
        public const int LeftIndex = 3;
        public const int BackIndex = 4;
        public const int RightIndex = 5;

        public static Dector3 Zero => new Dector3();
        public static Dector3 Up => new Dector3(0, 1, 0);
        public static Dector3 Down => new Dector3(0, -1, 0);
        public static Dector3 Forward => new Dector3(0, 0, 1);
        public static Dector3 Left => new Dector3(-1, 0, 0);
        public static Dector3 Back => new Dector3(0, 0, -1);
        public static Dector3 Right => new Dector3(1, 0, 0);

        public static Dector3[] Directions => _directions;

        private static Dector3[] _directions =
        {
            Up,
            Down,
            Forward,
            Left,
            Back,
            Right
        };

        public Dector3()
        {
        }

        public Dector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Dector3 Random(Random random, Dector3 minRandom, Dector3 maxRandom)
        {
            /*
             * randomize random random
             */
            int randomX = random.Next(minRandom.x, maxRandom.x);
            int randomY = random.Next(minRandom.y, maxRandom.y);
            int randomZ = random.Next(minRandom.z, maxRandom.z);
            /*
             * random x, random y and random z make random dector
             */
            Dector3 randomDector3 = new Dector3(randomX, randomY, randomZ);

            /*
             * return random dector
             */
            return randomDector3;
        }

        public static bool IsDifferentOnlyByOneAxis(Dector3 d1, Dector3 d2)
        {
            int differences = 0;
            if (d1.x != d2.x)
            {
                differences++;
            }

            if (d1.y != d2.y)
            {
                differences++;
            }

            if (d1.z != d2.z)
            {
                differences++;
            }

            return differences == 1;
        }

        public static (Dector3, Dector3) ToMinAndMax(Dector3 point1, Dector3 point2)
        {
            int fromX = Math.Min(point1.x, point2.x);
            int toX = Math.Max(point1.x, point2.x);

            int fromY = Math.Min(point1.y, point2.y);
            int toY = Math.Max(point1.y, point2.y);

            int fromZ = Math.Min(point1.z, point2.z);
            int toZ = Math.Max(point1.z, point2.z);

            return (new Dector3(fromX, fromY, fromZ), new Dector3(toX, toY, toZ));
        }


        public Dector3 ToOne()
        {
            Dector3 result = new Dector3();
            if (x != 0)
                result.x = x / Math.Abs(x);
            if (y != 0)
                result.y = y / Math.Abs(y);
            if (z != 0)
                result.z = z / Math.Abs(z);
            return result;
        }

        public static int Distance(Dector3 d1, Dector3 d2)
        {
            Dector3 delta = (d1 - d2).WithAbsAxis();
            return Math.Max(delta.x, Math.Max(delta.y, delta.z));
        }

        public static Dector3 GetDirection(int index)
        {
            return _directions[index];
        }

        public static int GetDirectionIndex(Dector3 direction)
        {
            for (int i = 0; i < _directions.Length; i++)
            {
                if (_directions[i].Equals(direction))
                    return i;
            }

            return -1;
        }

        public static Dector3 MultiplyCorrespondingAxis(Dector3 d1, Dector3 d2)
        {
            return new Dector3(d1.x * d2.x, d1.y * d2.y, d1.z * d2.z);
        }

        public Dector3 WithAbsAxis()
        {
            return new Dector3(Math.Abs(x), Math.Abs(y), Math.Abs(z));
        }

        public Dector3 Rotated(int rotation)
        {
            if (rotation < 0 || rotation > 3)
                throw new Exception($"Invalid rotation: {rotation}");
            /* at 0
             * z - forward
             * y - up
             * x - right
             */

            // TODO: keep eye on this, possible wrong calculations
            Dector3 ones = ToOne();
            Dector3 xVector = new Dector3(ones.x, 0, 0);
            Dector3 yVector = new Dector3(0, ones.y, 0);
            Dector3 zVector = new Dector3(0, 0, ones.z);

            int indexX = 2 + (GetDirectionIndex(xVector) + rotation) % 4;
            int indexY = 2 + (GetDirectionIndex(yVector) + rotation) % 4;
            int indexZ = 2 + (GetDirectionIndex(zVector) + rotation) % 4;

            Dector3 turnedX = GetDirection(indexX) * x;
            Dector3 turnedY = GetDirection(indexY) * y;
            Dector3 turnedZ = GetDirection(indexZ) * z;

            Dector3 result = turnedX + turnedY + turnedZ;

            return result;
        }

        public static Vector3 operator +(Dector3 d, Vector3 v)
        {
            return new Vector3(
                d.x + v.x,
                d.y + v.y,
                d.z + v.z
            );
        }

        public static Vector3 operator +(Vector3 v, Dector3 d)
        {
            return d + v;
        }

        public static Dector3 operator +(Dector3 d1, Dector3 d2)
        {
            return new Dector3(
                d1.x + d2.x,
                d1.y + d2.y,
                d1.z + d2.z
            );
        }

        public static Dector3 operator -(Dector3 d1, Dector3 d2)
        {
            return d1 + d2 * -1;
        }

        public static Dector3 operator *(Dector3 dector3, int multiplier)
        {
            dector3.x *= multiplier;
            dector3.y *= multiplier;
            dector3.z *= multiplier;
            return dector3;
        }

        public static Dector3 operator /(Dector3 d, int i)
        {
            return new Dector3(d.x / i, d.y / i, d.z / i);
        }

        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Dector3 other = (Dector3) obj;
            return other.x == x && other.y == y && other.z == z;
        }

        protected bool Equals(Dector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x;
                hashCode = (hashCode * 397) ^ y;
                hashCode = (hashCode * 397) ^ z;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"D({x}, {y}, {z})";
        }
    }
}