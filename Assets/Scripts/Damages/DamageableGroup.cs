using UnityEngine;

namespace Damages
{
    [CreateAssetMenu(fileName = "Damageable Group", menuName = "Skull Crusher/Damageable Group")]
    public class DamageableGroup: ScriptableObject
    {
        public float damageMultiplier = 1.0f;
    }
}