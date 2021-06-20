using UnityEngine;

namespace Weapons
{
    public class Ammo : MonoBehaviour
    {
        public int weaponPlayerSlot = 1;
        public int ammoAmount = 20;

        private void Start()
        {
            if (weaponPlayerSlot < 0 || weaponPlayerSlot > 9)
            {
                UnityEngine.Debug.LogError($"Invalid weaponPlayerSlot: {weaponPlayerSlot}", this);
            }

            if (ammoAmount < 0)
            {
                UnityEngine.Debug.LogError($"Invalid ammoAmount: {ammoAmount}", this);
            }
        }
    }
}