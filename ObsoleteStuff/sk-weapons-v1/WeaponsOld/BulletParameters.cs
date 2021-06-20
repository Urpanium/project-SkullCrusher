using System;
using UnityEngine;

namespace WeaponsOld
{
    [Serializable]
    public class BulletParameters
    {
        public float speed = 20.0f;
        public float radius = 0.02f;
        [Range(0, 1)] public float initialRicochetChance = 0.75f;
        [Range(0, 90)] public float maxRicochetAngle = 45.0f;
        public int defaultDamage;
        public int headDamage;
        public int armDamage;
        public int legDamage;
    }
}