using System;
using UnityEngine;

namespace Level.Covers.Classes.Util
{
    [Serializable]
    public class Vector3Data
    {
        public float x;
        public float y;
        public float z;

        public Vector3Data(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}