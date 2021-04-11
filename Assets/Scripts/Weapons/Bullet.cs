using UnityEngine;

namespace Weapons
{
    public class Bullet : MonoBehaviour
    {
        public BulletParameters parameters;
        // just an index of bullet in clip
        public int seed;
        public float flyingTime;
        public float ricochetChance;
    }
}