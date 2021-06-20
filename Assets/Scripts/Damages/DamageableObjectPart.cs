using UnityEngine;
using Util;
using Weapons.Damages;

namespace Damages
{
    [RequireComponent(typeof(BoxCollider))]
    public class DamageableObjectPart : MonoBehaviour
    {
        public DamageableGroup damageableGroup;
        
        private BoxCollider boxCollider;
        private DamageableObject mainObject;
        
        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (!mainObject)
            {
                UnityEngine.Debug.LogWarning("No main object were assigned for damageable part!", this);
            }
        }

        public void TakeDamage(Damage damage)
        {
            
        }

        public void SetMainObject(DamageableObject damageableObject)
        {
            mainObject = damageableObject;
        }

        private void OnDrawGizmos()
        {
            if (!damageableGroup)
                return;
            if (!boxCollider)
                boxCollider = GetComponent<BoxCollider>();
            Gizmos.color = ColorUtil.GetStringBasedColor(damageableGroup.name);
            DrawWireCubeGizmo(transform.position, transform.forward, transform.up, transform.right, boxCollider.size);
        }

        private void DrawWireCubeGizmo(Vector3 position, Vector3 forward, Vector3 up, Vector3 right, Vector3 size)
        {
            Vector3[] points =
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1),

                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1),
            };

            int[] lines =
            {
                // lower ring
                0, 1,
                1, 2,
                2, 3,
                3, 0,

                // upper ring
                4, 5,
                5, 6,
                6, 7,
                7, 4,

                // legs
                0, 4,
                1, 5,
                2, 6,
                3, 7,
            };

            for (int i = 0; i < lines.Length; i += 2)
            {
                int index1 = lines[i];
                int index2 = lines[i + 1];

                Vector3 point1 = points[index1];
                Vector3 point2 = points[index2];

                Vector3 offset1 = point1.x * right * size.x + point1.y * up * size.y + point1.z * forward * size.z;

                Vector3 offset2 = point2.x * right * size.x + point2.y * up * size.y + point2.z * forward * size.z;

                Vector3 basis = right * size.x + up * size.y + forward * size.z;
                Gizmos.DrawLine(position + offset1 - 0.5f * basis,
                    position + offset2 - 0.5f * basis);
            }
        }
    }
}