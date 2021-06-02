using System;

namespace Level.Generation.PathLayer.Path
{
    public class PathTile
    {
        /*
         * this is new PathNode, takes less memory
         * 
         * it doesn't know, were it is
         * because it doesn't need to
         *
         * made byte representation to
         * reduce memory usage
         */

        public bool CanGoToTop { get; set; }
        public bool CanGoToBottom { get; set; }
        public bool CanGoToForward { get; set; }
        public bool CanGoToLeft { get; set; }
        public bool CanGoToBack { get; set; }
        public bool CanGoToRight { get; set; }

        /*
         * defines if we can change access to tile
         * true when it's not a part of prototype (corridor, room)
         */
        public bool Changeable { get; set; }

        public const byte MaxByteValue = 128; // 2 ^ 7 for 6 sides and changeable bit

        public PathTile()
        {
        }

        public byte ToByte()
        {
            bool[] array =
            {
                CanGoToTop,
                CanGoToBottom,
                CanGoToForward,
                CanGoToLeft,
                CanGoToBack,
                CanGoToRight,
                Changeable
            };
            byte result = 0;
            foreach (var bit in array)
            {
                result *= 2;

                result |= (byte) (bit ? 1 : 0);
            }

            return result;
        }

        public static PathTile FromInt(int tile)
        {
            if (tile > 63 || tile < 0)
                throw new Exception("Invalid tile: " + tile);
            PathTile pathTile = new PathTile();
            bool[] array = new bool[6];

            int number = tile;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = number % 2 == 1;
                number /= 2;
            }


            for (int i = 0; i < array.Length; i++)
            {
                pathTile.SetDirectionAccess(i, array[i]);
                if (i == array.Length - 1)
                    pathTile.Changeable = array[i];
            }

            return pathTile;
        }


        public void SetDirectionAccess(int index, bool value)
        {
            if (index < 0 || index > 5)
                throw new Exception($"Invalid direction index: {index}");

            switch (index)
            {
                case 0:
                {
                    CanGoToTop = value;
                    break;
                }
                case 1:
                {
                    CanGoToBottom = value;
                    break;
                }
                case 2:
                {
                    CanGoToForward = value;
                    break;
                }
                case 3:
                {
                    CanGoToLeft = value;
                    break;
                }
                case 4:
                {
                    CanGoToBack = value;
                    break;
                }
                case 5:
                {
                    CanGoToRight = value;
                    break;
                }
            }
        }

        public bool GetDirectionAccess(int index)
        {
            if (index < 0 || index > 5)
                throw new Exception($"Invalid direction index: {index}");
            bool[] array =
            {
                CanGoToTop,
                CanGoToBottom,
                CanGoToForward,
                CanGoToLeft,
                CanGoToBack,
                CanGoToRight
            };
            return array[index];
        }

        public void AllowGoToAnyDirection()
        {
            CanGoToTop = true;
            CanGoToBottom = true;
            CanGoToForward = true;
            CanGoToLeft = true;
            CanGoToBack = true;
            CanGoToRight = true;
        }
    }
}