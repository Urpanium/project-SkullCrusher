using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponsOld
{
    public class DamageableObject : MonoBehaviour
    {
        public int health = 100;

        public BoxCollider[] heads;
        public BoxCollider[] arms;
        public BoxCollider[] legs;


        public void TakeDamage(Collider hitCollider, BulletParameters bulletParameters)
        {
            
            bool head = heads.Contains(hitCollider);
            bool arm = arms.Contains(hitCollider);
            bool leg = legs.Contains(hitCollider);
            if (head)
            {
                health -= bulletParameters.headDamage;
            }

            if (arm)
            {
                health -= bulletParameters.armDamage;
            }

            if (leg)
            {
                health -= bulletParameters.legDamage;
            }

            if (!leg && !arm && !head)
                health -= bulletParameters.defaultDamage;
            
            if (health <= 0)
                Die();
        }

        private void Die()
        {
            // TODO: make death manager
            // but for now just destroy object
            print(name + " died");
            Destroy(gameObject);
        }
        
    }
}