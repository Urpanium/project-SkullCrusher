using Character;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UI
{
    public class ClipAmmoText : MonoBehaviour
    {
        public WeaponController weaponController;

        private Text text;

        private void Start()
        {
            text = GetComponent<Text>();
        }

        private void Update()
        {
            Weapon currentWeapon = weaponController.GetMainWeapon();
            if (currentWeapon)
                text.text = currentWeapon.currentClipAmmoAmount + "/" +
                            currentWeapon.currentRemainedAmmoAmount;
            else
                text.text = "";
        }
    }
}