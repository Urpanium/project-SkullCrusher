using Character;
using Preferences;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        public Vector3 offset = new Vector3(0.35f, -0.47f, 0.60f);

        public Controller controller;
        public WeaponParameters weaponParameters;

        public BulletParameters bulletParameters;

        public UnityEvent fire1Event; // default fire
        public UnityEvent fire2Event; // alternative fire
        public UnityEvent reloadEvent;
        public UnityEvent onReloadStartedEvent;
        public UnityEvent onReloadedEvent;
        public UnityEvent onNewShotReadyEvent;

        // just empty transforms to represent position
        // and orientation of necessary weapon parts 
        public Transform muzzle;

        public global::Weapons.Shutter.Shutter shutter;

        // must point at direction in which shells will
        // be ejected 
        public Transform ejector;

        // will be spawned in ejector
        public Transform shellPrefab;
        public Transform bulletPrefab;

        // TODO: can attach rigidbody to it 
        public Transform clip;

        public float shellInitialVelocityMultiplier = 1.0f;
        public float shellInitialAngularVelocityMultiplier = 1.0f;


        public int currentClipAmmoAmount;
        public int currentRemainedAmmoAmount; // does not include currentClipAmmoAmount

        [Header("Gizmo settings")] public int gizmoShellEjectionTrajectorySamples = 120;

        /*
         * ================================PRIVATE================================
         */

        private BulletManager bulletManager;

        private float timePerShot;

        private float currentTimePerShot;

        //private bool reloading;
        private float currentReloadTime;

        private void Start()
        {
            bulletManager = GameObject.Find(Settings.GameObjects.GlobalController).GetComponent<BulletManager>();

            
            currentTimePerShot = timePerShot;

            currentClipAmmoAmount = weaponParameters.clipAmmoAmount;
            currentRemainedAmmoAmount = weaponParameters.totalAmmoAmount - weaponParameters.clipAmmoAmount;
        }

        private void Update()
        {
            //TODO: consider removing on release
            timePerShot = 1 / weaponParameters.shootRate;
            
            if (currentTimePerShot > 0)
            {
                currentTimePerShot -= Time.deltaTime;
                if (currentReloadTime <= 0)
                    OnNewShotReady();
            }

            if (currentReloadTime > 0)
            {
                currentReloadTime -= Time.deltaTime;
                if (currentReloadTime <= 0)
                    OnReloaded();
            }
        }

        public void StandardFire1()
        {
            if (!CanFire())
                return;
            
            // decrease clip ammo amount
            currentClipAmmoAmount--;
            // restart the timer
            currentTimePerShot = timePerShot;
            
            shutter.OnShoot();
            
            fire1Event.Invoke();

            Transform bulletTransform = MakeBullet();
            Bullet bullet = bulletTransform.gameObject.AddComponent<Bullet>();
            bullet.parameters = bulletParameters;
            bullet.seed = currentClipAmmoAmount;
            bullet.ricochetChance = bulletParameters.initialRicochetChance;
            //bullet.ricochetChance = bulletParameters.initialRicochetChance;
            //bullet.maxRicochetAngle = bulletParameters.maxRicochetAngle;
            //TODO: check this
            bulletManager.AddBullet(bullet);
        }

        private Transform MakeBullet()
        {
            return Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        }
        

        public void StandardFire2()
        {
            //TODO: Remove?
            // just do nothing 
        }

        public void StandardReload()
        {
            if (currentReloadTime <= 0)
            {
                currentReloadTime = weaponParameters.reloadTime;
                // start reloading animation
                onReloadStartedEvent.Invoke();
            }
        }

        private void OnNewShotReady()
        {
            // TODO: remove?
            onNewShotReadyEvent.Invoke();
        }

        private void OnReloaded()
        {
            onReloadedEvent.Invoke();
        }

        public void EjectShell()
        {
            // shell spawn and ejection
            Transform shell = Instantiate(shellPrefab, ejector.position, Random.rotation);
            Rigidbody shellRigidbody = shell.GetComponent<Rigidbody>();
            shellRigidbody.velocity = shellInitialVelocityMultiplier * shellRigidbody.mass * ejector.forward +
                                      controller.GetVelocity();
            shellRigidbody.rotation = ejector.rotation;
            shellRigidbody.angularVelocity =
                Random.onUnitSphere * shellRigidbody.mass * shellInitialAngularVelocityMultiplier;
        }


        private bool CanFire()
        {
            return currentClipAmmoAmount > 0 && currentTimePerShot <= 0;
        }


        private void OnDrawGizmos()
        {
            // draw muzzle position and try to raycast

            if (muzzle)
            {
                Gizmos.color = Color.red;
                Vector3 position = muzzle.position;
                //Gizmos.DrawSphere(position, 0.03125f);
                Ray ray = new Ray(muzzle.position, muzzle.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Gizmos.DrawLine(position, hit.point);
                    Gizmos.DrawSphere(hit.point, 0.0625f);
                    // draw normal
                    // Gizmos.DrawLine(hit.point, hit.point + hit.normal);
                    // draw possible ricochet direction
                    if (hit.collider.gameObject.tag.Equals(Settings.Tags.Ricochetable))
                    {
                        Vector3 ricochetedDirection = Quaternion.AngleAxis(180, hit.normal) * muzzle.forward * -1;
                        Gizmos.DrawLine(hit.point, hit.point + ricochetedDirection);
                    }
                }
                else
                {
                    Gizmos.DrawLine(position, position + muzzle.forward);
                }
            }

            // draw ejector position and direction
            if (ejector)
            {
                Gizmos.color = Color.green;

                Vector3 position = ejector.position;
                Vector3 velocity = ejector.forward * shellInitialVelocityMultiplier;
                float samplesInverted = 1.0f / gizmoShellEjectionTrajectorySamples;

                Gizmos.DrawSphere(position, 0.03125f);

                while (position.y + 1 > transform.position.y)
                {
                    Vector3 nextPosition = position + velocity * samplesInverted;
                    Gizmos.DrawLine(position, nextPosition);
                    velocity += Physics.gravity * samplesInverted;
                    position = nextPosition;
                }
            }
        }
    }
}