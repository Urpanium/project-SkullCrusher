using System;
using UnityEngine;
using UnityEngine.Events;
using Weapon;
using Weapon.Shutter;

namespace Weapons.Classes
{
    public class Weapon : MonoBehaviour
    {
        public WeaponParameters weaponParameters;

        public BulletDamageParameters bulletDamageParameters;

        public UnityEvent fire1Event; // default fire
        public UnityEvent fire2Event; // alternative fire
        public UnityEvent reloadEvent;

        // just empty transforms to represent position
        // and orientation of necessary weapon parts 
        public Transform muzzle;

        public Shutter shutter;

        // must point at direction in which shells will
        // be ejected 
        public Transform ejector;

        // will be spawned in ejector
        public Transform shellPrefab;
        
        public float shellInitialVelocityMultiplier = 1.0f;


        public int currentClipAmmoAmount;
        public int currentRemainedAmmoAmount; // does not include currentClipAmmoAmount

        [Header("Gizmo settings")] public int gizmoEjectionTrajectorySamples = 120;
        /*public float gizmoEjectionTrajectoryTime = 0.25f;*/

        private float timePerShot;

        private float currentTimePerShot;

        private void Start()
        {
            timePerShot = weaponParameters.clipShootOutTime / weaponParameters.clipAmmoAmount;
            currentTimePerShot = timePerShot;

            currentClipAmmoAmount = weaponParameters.clipAmmoAmount;
            currentRemainedAmmoAmount = weaponParameters.totalAmmoAmount - weaponParameters.clipAmmoAmount;
        }

        private void Update()
        {
            if (currentTimePerShot > 0)
            {
                currentTimePerShot -= Time.deltaTime;
            }
        }

        public void StandardFire1()
        {
            if (!CanFire())
                return;
            // raycast bullet

            

            // shutter slide
            

            // values update
            // decrease clip ammo amount
            currentClipAmmoAmount--;
            // restart the timer
            currentTimePerShot = timePerShot;
            fire1Event.Invoke();
        }

        public void EjectShell()
        {
            // shell spawn and ejection
            Transform shell = Instantiate(shellPrefab);
            Rigidbody shellRigidbody = shell.GetComponent<Rigidbody>();
            shellRigidbody.velocity = ejector.forward * shellInitialVelocityMultiplier;
        }

        public void StandardFire2()
        {
            // just do nothing 
        }

        public void StandardReload()
        {
            
        }
        
        private bool CanFire()
        {
            return currentClipAmmoAmount > 0 && currentTimePerShot <= 0;
        }


        private void OnDrawGizmos()
        {
            // draw muzzle position and direction

            if (muzzle)
            {
                Gizmos.color = Color.red;
                Vector3 position = muzzle.position;
                Gizmos.DrawSphere(position, 0.03125f);
                Gizmos.DrawLine(position, position + muzzle.forward);
            }
            
            // draw ejector position and direction
            if (ejector)
            {
                Gizmos.color = Color.green;
                
                Vector3 position = ejector.position;
                Vector3 velocity = ejector.forward * shellInitialVelocityMultiplier;
                float samplesInverted = 1.0f / gizmoEjectionTrajectorySamples;
                
                Gizmos.DrawSphere(position, 0.03125f);
                
                while (position.y + 1 > transform.position.y)
                {
                    Vector3 nextPosition = position + velocity * samplesInverted;
                    Gizmos.DrawLine(position, nextPosition);
                    velocity += Physics.gravity * samplesInverted;
                    position = nextPosition;
                }
                /*Gizmos.DrawSphere(position, 0.03125f);
                float samplesInverted = gizmoEjectionTrajectoryTime/ gizmoEjectionTrajectorySamples;
                for (int i = 0; i < gizmoEjectionTrajectorySamples; i++)
                {
                    Vector3 nextPosition = position + (ejector.forward * shellInitialVelocityMultiplier + Physics.gravity * i) * i * samplesInverted;
                    Gizmos.DrawLine(position, nextPosition);
                    position = nextPosition;
                }*/
            }
            
        }
    }
}