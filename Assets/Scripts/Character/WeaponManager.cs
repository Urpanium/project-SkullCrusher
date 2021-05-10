using System.Collections.Generic;
using Preferences;
using SingleInstance;
using UnityEngine;
using Weapons;

namespace Character
{
    public class WeaponManager : MonoBehaviour
    {
        
        public Transform cameraTransform;


        private InputManagerOld inputManager;

        private List<Weapon> weapons;
        
        private Weapon currentWeapon;
        private Transform currentWeaponTransform;
        private BoxCollider currentWeaponBoxCollider;
        private Rigidbody currentWeaponRigidbody;


        private void Start()
        {
            inputManager = GameObject.Find(Settings.GameObjects.GlobalController).GetComponent<InputManagerOld>();
        }

        private void Update()
        {
            if (currentWeaponTransform)
            {
                // TODO: remove?
                currentWeaponTransform.rotation = cameraTransform.rotation;
            }

            if (inputManager.isFire1Pressed)
            {
                if (currentWeapon)
                {
                    currentWeapon.fire1Event.Invoke();
                }
            }

        }

        private void PickupWeapon(Transform weaponTransform)
        {
            weaponTransform.parent = cameraTransform;

            currentWeaponTransform = weaponTransform;
            currentWeapon = weaponTransform.GetComponent<Weapon>();
            currentWeapon.isEquipped = true;
            currentWeapon.isEquippedByPlayer = true;
            currentWeaponBoxCollider = weaponTransform.GetComponent<BoxCollider>();
            currentWeaponRigidbody = weaponTransform.GetComponent<Rigidbody>();

            currentWeaponRigidbody.isKinematic = true;
            
            weaponTransform.localPosition = currentWeapon.offset;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(Settings.Tags.Weapon) && !other.transform.GetComponent<Weapon>().isEquipped)
            {
                PickupWeapon(other.transform);
            }
        }

    }
}