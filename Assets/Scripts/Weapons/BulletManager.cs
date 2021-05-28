using System;
using System.Collections.Generic;
using Preferences;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons
{
    [RequireComponent(typeof(SphereCollider))]
    public class BulletManager : MonoBehaviour
    {
        public LayerMask shootableMask;
        public float bulletTimeout = 60.0f;

        private List<Bullet> bullets;

        private void Start()
        {
            bullets = new List<Bullet>();
        }


        private void Update()
        {
            //TODO: update bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                bool destroyed = UpdateBullet(bullets[i]);
                if (destroyed)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
            }
        }


        private bool UpdateBullet(Bullet bullet)
        {
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
            RaycastHit hit;

            if (Physics.SphereCast(ray, bullet.parameters.radius, out hit, distance, shootableMask))
            {
                Vector3 normal = hit.normal;
                // - 90 because of normal
                float angle = Vector3.Angle(normal, direction) - 90;
                // if we hit something that can ricochet our bullet
                // and angle fits
                // TODO: also add hitting enemy / player case
                string hitObjectTag = hit.transform.tag;
                if (hitObjectTag.Equals(Settings.Tags.Ricochetable))
                {
                    Random.InitState(bullet.seed);

                    float chance = bullet.ricochetChance *
                                   ((bullet.parameters.maxRicochetAngle - angle) /
                                    bullet.parameters.maxRicochetAngle);
                    // try ricochet
                    if (Random.value < chance &&
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
                    // we hit something (body)
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    DamageableObject damageableObject = hit.transform.root.GetComponent<DamageableObject>();
                    if (damageableObject)
                    {
                        damageableObject.TakeDamage(hit.collider, bullet.parameters);
                    }

                    //TODO: show some particles and destroy bullet
                    Destroy(bullet.gameObject);
                    return true;
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


        public void AddBullet(Bullet bullet)
        {
            bullets.Add(bullet);
        }
    }
}