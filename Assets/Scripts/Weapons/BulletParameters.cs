using System;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public class BulletParameters
    {
        public float speed = 20.0f;
        public float radius = 0.02f;
        public LayerMask shootableMask; 
        public int defaultDamage;
        public int headDamage;
        public int armDamage;
        public int legDamage;
    }
}