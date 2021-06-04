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

        private static readonly Dector3 ZeroDector = new Dector3();
        private static readonly Dector3 UpDector = new Dector3(0, 1, 0);
        private static readonly Dector3 DownDector = new Dector3(0, -1, 0);
        private static readonly Dector3 ForwardDector = new Dector3(0, 0, 1);
        private static readonly Dector3 LeftDector = new Dector3(-1, 0, 0);
        private static readonly Dector3 BackDector = new Dector3(0, 0, -1);
        private static readonly Dector3 RightDector = new Dector3(1, 0, 0);

        public static Dector3 Zero => ZeroDector;
        public static Dector3 Up => UpDector;
        public static Dector3 Down => DownDector;
        public static Dector3 Forward => ForwardDector;
        public static Dector3 Left => LeftDector;
        public static Dector3 Back => BackDector;
        public static Dector3 Right => RightDector;

        public static readonly Dector3[] Directions =
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

        public Dector3(Dector3 d)
        {
            x = d.x;
            y = d.y;
            z = d.z;
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
            if (x > 0)
                result.x = 1;
            if (x < 0)
                result.x = -1;

            if (y > 0)
                result.y = 1;
            if (y < 0)
                result.y = -1;

            if (z > 0)
                result.z = 1;
            if (z < 0)
                result.z = -1;

            return result;
        }

        public static int Distance(Dector3 d1, Dector3 d2)
        {
            Dector3 delta = (d1 - d2).WithAbsAxis();
            return Math.Max(delta.x, Math.Max(delta.y, delta.z));
        }

        /*public static Dector3[] Directions()
        {
            return new[]
            {
                Up,
                Down,
                Forward,
                Left,
                Back,
                Right
            };
        }*/

        public static Dector3 GetDirection(int index)
        {
            return Directions[index].ToOne();
        }


        public static int GetDirectionIndex(Dector3 direction)
        {
            // это были степени отчаяния и безумия, ага

            /*if (direction.Equals(new Dector3(0, 1, 0)))
                return 0;
            if (direction.Equals(new Dector3(0, -1, 0)))
                return 1;
            if (direction.Equals(new Dector3(0, 0, 1)))
                return 2;
            if (direction.Equals(new Dector3(-1, 0, 0)))
                return 3;
            if (direction.Equals(new Dector3(0, 0, -1)))
                return 4;
            if (direction.Equals(new Dector3(1, 0, 0)))
                return 5;
            return -1;*/

            /*if (direction.Equals(Up))
                return 0;
            if (direction.Equals(Down))
                return 1;
            if (direction.Equals(Forward))
                return 2;
            if (direction.Equals(Left))
                return 3;
            if (direction.Equals(Back))
                return 4;
            if (direction.Equals(Right))
                return 5;
            return -1;*/

            //UnityEngine.Debug.Log($"GDI: got {direction}");
            for (int i = 0; i < Directions.Length; i++)
            {
                Dector3 d = Directions[i];
                //UnityEngine.Debug.Log($"GDI: checking {d} (index {i})");
                if (
                    direction.x == d.x
                    && direction.y == d.y
                    && direction.z == d.z
                )
                {
                    //UnityEngine.Debug.Log($"GDI: returning {i}");
                    return i;
                }

                if (direction.Equals(Directions[i]))
                    return i;
            }

            //UnityEngine.Debug.Log($"GDI: returning -1");
            return -1;
        }

        public static void DirCheck()
        {
            UnityEngine.Debug.Log("DIRECTION CHECK: ");
            foreach (Dector3 direction in Directions)
            {
                UnityEngine.Debug.Log($"{direction} V = {(Vector3) direction}");
            }
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
            return new Dector3(
                d1.x - d2.x,
                d1.y - d2.y,
                d1.z - d2.z
            );
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

        public static implicit operator Vector3(Dector3 d)
        {
            return new Vector3(d.x, d.y, d.z);
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
            return $"D ({x}, {y}, {z})";
        }
    }
}