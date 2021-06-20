using System.Collections.Generic;
using Damages;
using Preferences;
using UnityEngine;
using Weapons;
using Weapons.Classes;
using Weapons.Parameters;
using Random = System.Random;

namespace SingleInstance
{
    public class BulletManager : MonoBehaviour
    {
        public LayerMask shootableMask;
        public float bulletTimeout = 10.0f;

        // store bullets as childs of object for convenience
        private Transform bulletsTransform;
        private List<Bullet> bullets;

        private void Start()
        {
            bullets = new List<Bullet>();
            // пельмени
            GameObject bulletsGameObject = new GameObject("Bullets");
            bulletsTransform = bulletsGameObject.transform;
        }


        private void Update()
        {
            //TODO: update bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                Bullet bullet = bullets[i];
                bool destroyed = UpdateBullet(bullets[i]);
                if (destroyed)
                {
                    bullets.Remove(bullet);
                }
            }
        }


        private bool UpdateBullet(Bullet bullet)
        {
            // checking timeout
            bullet.flyingTime += Time.deltaTime;
            if (bullet.flyingTime > bulletTimeout)
            {
                bullets.Remove(bullet);
                Destroy(bullet.gameObject);
                return true;
            }

            Transform bulletTransform = bullet.transform;

            Vector3 direction = bulletTransform.forward;

            float distance = bullet.parameters.speed * Time.deltaTime;

            Vector3 offset = distance * direction;
            //bulletTransform.position += offset;

            // cast a ray to check if we skipping some object
            // this check must be made after we calculate next bullet position
            // fucked this up and lost hours to figure it out
            Ray ray = new Ray(bulletTransform.position, direction);

            if (Physics.SphereCast(ray, bullet.parameters.radius, out RaycastHit hit, distance, shootableMask))
            {
                Vector3 normal = hit.normal;
                // - 90 because of normal
                float angle = Vector3.Angle(normal, direction) - 90;
                // if we hit something that can ricochet our bullet
                // and angle fits
                bool shouldRicochet = hit.transform.CompareTag(Settings.Tags.Ricochetable);

                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                DamageableObjectPart damageableObjectPart = hit.transform.GetComponent<DamageableObjectPart>();
                if (damageableObjectPart)
                {
                    damageableObjectPart.TakeDamage(new Damage(bullet.parameters.damageAmount));
                }

                if (!shouldRicochet)
                {
                    Destroy(bullet.gameObject);
                    return true;
                }

                // we hit something ricochetable

                Random random = new Random(bullet.seed);

                float chance = bullet.ricochetChance *
                               ((bullet.parameters.maxRicochetAngle - angle) /
                                bullet.parameters.maxRicochetAngle);
                // try ricochet
                if (random.NextDouble() < chance &&
                    angle <= bullet.parameters.maxRicochetAngle)
                {
                    bulletTransform.forward = Quaternion.AngleAxis(180, hit.normal) * bulletTransform.forward * -1;
                    // push bullet out of plane a little bit
                    bulletTransform.position = hit.point + hit.normal * bullet.parameters.radius;
                    return false;
                }
            }
            else
            {
                // move bullet
                bulletTransform.position += offset;
                return false;
            }


            return false;
        }


        public void AddBullet(Transform bulletTransform, BulletParameters parameters, int seed, float ricochetChance)
        {
            // you are my son now
            bulletTransform.parent = bulletsTransform;

            Bullet bullet = bulletTransform.gameObject.AddComponent<Bullet>();

            bullet.parameters = parameters;
            bullet.seed = seed;
            bullet.ricochetChance = ricochetChance;

            bullets.Add(bullet);
        }
    }
}