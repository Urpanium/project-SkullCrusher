using UnityEngine;
using Random = System.Random;

namespace Util
{
    public static class ColorUtil
    {

        // NOTE: does not give a fuck about alpha
        public static Color Normalize(this Color c)
        {
            float length = Mathf.Sqrt(c.r * c.r + c.g * c.g + c.b * c.b);
            Color color = new Color(c.r / length, c.g / length, c.b / length);
            return color;
            
        }
        public static Color GetStringBasedColor(string s)
        {
            int hashSeed = s.GetHashCode();

            Random random = new Random(hashSeed);

            int red = 1 + random.Next(256);
            int green = 1 + random.Next(256);
            int blue = 1 + random.Next(256);

            red -= red % 16;
            green -= green % 16;
            blue -= blue % 16;

            return new Color(
                red / 256.0f,
                green / 256.0f,
                blue / 256.0f
            );
        }
    }
}