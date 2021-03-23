using UnityEngine;

namespace Weapons
{
    public class Bullet : MonoBehaviour
    {
        public BulletParameters parameters;
        // just an index of bullet in clip
        public int seed;
        [Range(0, 90)] public float maxRicochetAngle = 45.0f;
        [Range(0, 1.0f)] public float ricochetChance = 1.0f;
    }
}