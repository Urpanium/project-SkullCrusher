using Damages;

namespace Weapons.Damages
{
    public interface IDamageModificator
    {
        public void OnDamageReceived(Damage damage, DamageableGroup group);
    }
}