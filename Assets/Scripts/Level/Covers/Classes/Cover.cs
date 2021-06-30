using System;
using UnityEngine;

namespace Level.Covers.Classes
{
    [Serializable]
    public class Cover
    {
        public Vector3 position;
        public Vector3 direction;
        public CoverType type;

        public Cover(Vector3 position, Vector3 direction, CoverType type)
        {
            this.position = position;
            this.direction = direction;
            this.type = type;
        }
        
    }
}