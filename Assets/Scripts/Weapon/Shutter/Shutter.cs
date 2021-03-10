using System;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

namespace Weapon.Shutter
{
    public class Shutter : MonoBehaviour
    {
        public ShutterParameters shutterParameters;

        public UnityEvent onRollBack;
        
        // [0..1]
        private float shutterPositionValue = 0.0f;

        private float rollBackSpeed;
        private float resetSpeed;

        private bool rollingBack;

        private Vector3 initialPosition;
        private Vector3 shutterPositionInFullRollBack;

        private void Start()
        {
            initialPosition = /*transform.position*/ transform.localPosition;
            shutterPositionInFullRollBack = /*initialPosition + */transform.forward * shutterParameters.slideDistance * -1;

            // prepare inverted values because multiplication works faster
            rollBackSpeed = 1 / shutterParameters.rollBackTime;
            resetSpeed = 1 / shutterParameters.resetTime;
        }

        private void Update()
        {
            if (rollingBack)
                shutterPositionValue += Time.deltaTime * rollBackSpeed;
            else
                shutterPositionValue -= Time.deltaTime * resetSpeed;

            shutterPositionValue = Mathf.Clamp01(shutterPositionValue);

            transform.localPosition =
                Vector3.Lerp(initialPosition, shutterPositionInFullRollBack, shutterPositionValue);

            if (shutterPositionValue >= 1 - Mathf.Epsilon)
            {
                OnRollBacked();
            }
        }

        private void OnRollBacked()
        {
            // start reset of the shutter
            rollingBack = false;
            // invoke event to call all the actions that must be performed by other scripts
            onRollBack.Invoke();
        }

        public void OnShoot()
        {
            // roll back the shutter
            rollingBack = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawMesh(GetComponent<MeshFilter>().mesh, transform.position + transform.forward * shutterParameters.slideDistance * -1);
        }
    }
}