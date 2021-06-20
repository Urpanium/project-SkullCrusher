using System;

namespace WeaponsOld
{
    [Serializable]
    public class WeaponParameters
    {
        public int clipAmmoAmount; // currently in weapon
        public int totalAmmoAmount; // TotalAmmo (including those that are in clip)
        public float shootRate;
        public float reloadTime;
    }
}