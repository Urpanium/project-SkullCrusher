using System;

namespace Weapon
{
    [Serializable]
    public class WeaponParameters
    {
        public int clipAmmoAmount; // currently in weapon
        public int totalAmmoAmount; // TotalAmmo (including those that are in clip)
        public float clipShootOutTime;
        public float reloadTime;
    }
}