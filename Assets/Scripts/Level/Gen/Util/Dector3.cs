using System;

namespace WaveFunctionCollapse3D.Util
{
    public class Dector3
    {
        // Discrete Vector3
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }


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
            X = x;
            Y = y;
            Z = z;
        }

        public static bool IsDifferentOnlyByOneAxis(Dector3 d1, Dector3 d2)
        {
            int differences = 0;
            if (d1.X != d2.X)
            {
                differences++;
            }

            if (d1.Y != d2.Y)
            {
                differences++;
            }

            if (d1.Z != d2.Z)
            {
                differences++;
            }

            return differences == 1;
        }

        public static (Dector3, Dector3) ToMinAndMax(Dector3 point1, Dector3 point2)
        {
            int fromX = Math.Min(point1.X, point2.X);
            int toX = Math.Max(point1.X, point2.X);

            int fromY = Math.Min(point1.Y, point2.Y);
            int toY = Math.Max(point1.Y, point2.Y);

            int fromZ = Math.Min(point1.Z, point2.Z);
            int toZ = Math.Max(point1.Z, point2.Z);

            return (new Dector3(fromX, fromY, fromZ), new Dector3(toX, toY, toZ));
        }


        public Dector3 ToOne()
        {
            Dector3 result = new Dector3();
            if (X != 0)
                result.X = X / Math.Abs(X);
            if (Y != 0)
                result.Y = Y / Math.Abs(Y);
            if (Z != 0)
                result.Z = Z / Math.Abs(Z);
            return result;
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
            return new Dector3(d1.X * d2.X, d1.Y * d2.Y, d1.Z * d2.Z);
        }

        public Dector3 WithAbsAxis()
        {
            return new Dector3(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
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
            Dector3 xVector = new Dector3(ones.X, 0, 0);
            Dector3 yVector = new Dector3(0, ones.Y, 0);
            Dector3 zVector = new Dector3(0, 0, ones.Z);

            int indexX = 2 + (GetDirectionIndex(xVector) + rotation) % 4;
            int indexY = 2 + (GetDirectionIndex(yVector) + rotation) % 4;
            int indexZ = 2 + (GetDirectionIndex(zVector) + rotation) % 4;

            Dector3 turnedX = GetDirection(indexX) * X;
            Dector3 turnedY = GetDirection(indexY) * Y;
            Dector3 turnedZ = GetDirection(indexZ) * Z;
            
            Dector3 result = turnedX + turnedY + turnedZ;
            
            return result;
        }

        public static Dector3 operator +(Dector3 d1, Dector3 d2)
        {
            return new Dector3(
                d1.X + d2.X,
                d1.Y + d2.Y,
                d1.Z + d2.Z
            );
        }

        public static Dector3 operator -(Dector3 d1, Dector3 d2)
        {
            return d1 + d2 * -1;
        }

        public static Dector3 operator *(Dector3 dector3, int multiplier)
        {
            dector3.X *= multiplier;
            dector3.Y *= multiplier;
            dector3.Z *= multiplier;
            return dector3;
        }

        public override string ToString()
        {
            return $"D({X}, {Y}, {Z})";
        }
    }
}