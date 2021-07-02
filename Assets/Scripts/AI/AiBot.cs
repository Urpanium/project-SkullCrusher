using AI.Classes.Groups;
using AI.Classes.States;
using AI.Enums;
using AI.Scriptables;
using Damages;
using Preferences;
using UnityEngine;
using Weapons;

namespace AI
{
    [RequireComponent(typeof(AiController))]
    public class AiBot: MonoBehaviour
    {
        public AiBotConfig config;
        public AiBotState initialState;
        public Weapon initialWeapon;
        
        public AiBotGroupRole groupRole;
        public AiController controller;
        
        private AiManager aiManager;
        private AiGroup group;
        private AiState state;
        
        private Transform playerTransform;
        private DamageableObject damageableObject;
        private Weapon equippedWeapon;

        private Vector3 lastPlayerPosition;
        
        private void Awake()
        {
            controller = GetComponent<AiController>();
        }

        private void Start()
        {
            GameObject globalController = GameObject.Find(Settings.GameObjects.GlobalController);

            aiManager = globalController.GetComponent<AiManager>();
            controller = GetComponent<AiController>();
            if (initialWeapon)
                EquipWeapon(initialWeapon);
            /*
             * group = null
             * groupRole = AiBotGroupRole.Single
             */
        }

        private void Update()
        {
            state.Update();
            string nextState = state.TransitionCheck();
            if (!string.IsNullOrWhiteSpace(nextState))
            {
                // TODO: switch state
            }
        }

        private void EquipWeapon(Weapon weapon)
        {
            // TODO: implement EquipWeapon()
        }
        
        public void Fire()
        {
            // TODO: implement Fire()
        }

        public void ReloadWeapon()
        {
            // TODO: implement ReloadWeapon()
        }

        public bool IsInGroup()
        {
            return groupRole == AiBotGroupRole.Single;
        }

        public bool IsNeedToReload()
        {
            return equippedWeapon.currentClipAmmoAmount <= 0;
        }

        public void TrackPlayer()
        {
            lastPlayerPosition = playerTransform.position;
        }

        public Vector3 GetPlayerLastPosition()
        {
            return lastPlayerPosition;
        }
        
    }
}