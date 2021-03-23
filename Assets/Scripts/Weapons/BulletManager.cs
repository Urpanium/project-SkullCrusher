using Preferences;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(SphereCollider))]
    public class BulletManager : MonoBehaviour
    {
        public LayerMask shootableMask;


        private void Update()
        {
            //TODO: update bullets
        }

        private void UpdateBullet(Bullet bullet)
        {
            Transform bulletTransform = bullet.transform;

            Vector3 direction = bulletTransform.forward;
            float distance = bullet.parameters.speed * Time.deltaTime;

            // cast a ray to check if we skipping some object
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;
            if (Physics.SphereCast(ray, bullet.parameters.radius, out hit, distance, shootableMask))
            {
                Vector3 normal = hit.normal;
                float angle = Vector3.Angle(normal, direction);
                // if we hit something that can ricochet our bullet
                // and angle fits
                // TODO: also add hitting enemy / player case
                if (hit.transform.tag.Equals(Settings.Tags.Ricochetable) &&
                    angle <= bullet.maxRicochetAngle)
                {
                    Random.InitState(bullet.seed);

                    float chance = bullet.ricochetChance *
                                   (1 - (bullet.maxRicochetAngle - angle) / bullet.maxRicochetAngle);
                    if (Random.value < chance)
                    {
                        bulletTransform.forward = Quaternion.AngleAxis(90, hit.normal) * bulletTransform.forward;
                    }
                    else
                    {
                        //TODO: show some particles and destroy bullet
                        Destroy(bullet.transform);
                    }
                }
            }
            else
            {
                Vector3 offset = distance * direction;
                bulletTransform.position += offset;
            }
        }
    }
}