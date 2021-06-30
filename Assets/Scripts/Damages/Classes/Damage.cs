using Damages.Enums;

namespace Damages
{
    public class Damage
    {
        public DamageType type;
        public float amount;

        public Damage(float amount, DamageType type = DamageType.Bullet)
        {
            this.amount = amount;
            this.type = type;
        }
        
    }
}