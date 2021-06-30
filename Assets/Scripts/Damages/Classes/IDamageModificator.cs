namespace Damages.Classes
{
    public interface IDamageModificator
    {
        public void OnDamageReceived(Damage damage, DamageableGroup group);
    }
}