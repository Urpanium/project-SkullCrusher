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
        public float rotationSpeed = 30.0f;
        public float weaponCheckDistance = 3000.0f;
            
        private InputManagerOld inputManager;
        private BulletManager bulletManager;
        private List<Weapon> weapons;
        
        private Weapon currentWeapon;
        private Transform currentWeaponTransform;
        private BoxCollider currentWeaponBoxCollider;
        private Rigidbody currentWeaponRigidbody;


        private void Start()
        {
            GameObject globalController = GameObject.Find(Settings.GameObjects.GlobalController);
            inputManager = globalController.GetComponent<InputManagerOld>();
            bulletManager = globalController.GetComponent<BulletManager>();
        }

        private void Update()
        {
            
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
            weaponTransform.forward = cameraTransform.forward;
            currentWeaponTransform = weaponTransform;
            currentWeapon = weaponTransform.GetComponent<Weapon>();
            currentWeapon.isEquipped = true;
            currentWeapon.isEquippedByPlayer = true;
            currentWeaponBoxCollider = weaponTransform.GetComponent<BoxCollider>();
            currentWeaponRigidbody = weaponTransform.GetComponent<Rigidbody>();

            currentWeaponRigidbody.isKinematic = true;
            
            weaponTransform.localPosition = currentWeapon.offset;
        }

        public Weapon GetCurrentWeapon()
        {
            return currentWeapon;
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