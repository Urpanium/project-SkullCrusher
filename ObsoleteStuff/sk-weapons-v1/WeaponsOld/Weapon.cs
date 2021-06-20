using Character;
using Preferences;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace WeaponsOld
{
    public class Weapon : MonoBehaviour
    {
        public bool isEquipped = false;
        public bool isEquippedByPlayer = false;
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

        public global::WeaponsOld.Shutter.Shutter shutter;

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
        private WeaponManager weaponManager;

        private float timePerShot;
        private float currentTimePerShot;
        private float currentReloadTime;
        
        

        private void Start()
        {
            GameObject globalController = GameObject.Find(Settings.GameObjects.GlobalController);
            bulletManager = globalController.GetComponent<BulletManager>();
            
            controller = GameObject.FindGameObjectWithTag(Settings.Tags.Player).GetComponent<Controller>();
            weaponManager = controller.GetComponent<WeaponManager>();

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
            
            if (currentClipAmmoAmount == 0 && currentReloadTime <= 0)
            {
                StandardReload();
            }
        }

        public void StandardFire()
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
            if (isEquippedByPlayer)
            {
                bulletTransform.forward = controller.GetLookPoint(bulletManager.shootableMask, weaponManager.weaponCheckDistance) - muzzle.position;
            }
            
            Bullet bullet = bulletTransform.gameObject.AddComponent<Bullet>();
            bullet.parameters = bulletParameters;
            bullet.seed = currentClipAmmoAmount;
            bullet.ricochetChance = bulletParameters.initialRicochetChance;
            
            //TODO: check this
            bulletManager.AddBullet(bullet);

            if (currentClipAmmoAmount == 0)
            {
                StandardReload();
            }
        }

        private Transform MakeBullet()
        {
            return Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        }


        public void StandardReload()
        {
            int availableAmmo = GetAvailableForReloadAmmo();
            if (availableAmmo > 0 && currentReloadTime <= 0)
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
            int ammo = GetAvailableForReloadAmmo();
            if (ammo > 0)
            {
                currentClipAmmoAmount = ammo;
                currentRemainedAmmoAmount -= ammo;
                OnNewShotReady();
            }
        }

        private int GetAvailableForReloadAmmo()
        {
            int need = weaponParameters.clipAmmoAmount - currentClipAmmoAmount;
            int available = Mathf.Min(currentRemainedAmmoAmount, weaponParameters.clipAmmoAmount);
            return Mathf.Min(available, need);
        }

        public void EjectShell()
        {
            // shell spawn and ejection
            Transform shell = Instantiate(shellPrefab, ejector.position, Quaternion.LookRotation(ejector.right, ejector.up));
            Rigidbody shellRigidbody = shell.GetComponent<Rigidbody>();
            shellRigidbody.velocity = shellInitialVelocityMultiplier * shellRigidbody.mass * ejector.forward +
                                      (isEquippedByPlayer ? controller.GetVelocity() : Vector3.zero);
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