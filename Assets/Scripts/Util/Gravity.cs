using UnityEngine;

namespace Util
{
    public class Gravity
    {

        public static Vector3 down { get; private set; }
        //public static Vector3 forward { get; private set; }

        public static Vector3 up
        {
            get { return -down; }
        }

        static Gravity()
        {
            down = Vector3.down;
            //forward = Vector3.forward;
        }

        public static void Set(Vector3 newDown)
        {
            /*Vector3 axis = Vector3.Cross(down, newDown);
            float angle = Vector3.Angle(down, newDown);*/

            down = newDown;
            //forward = Quaternion.AngleAxis(angle, axis) * forward;
        }
    }
}