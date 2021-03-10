using UnityEngine;

namespace SingleInstance
{
    public class InputManagerOld : MonoBehaviour
    {
        [Header("Read-only")] public Vector3 move;
        public Vector3 look;

        public bool isCrouchPressed;
        public bool isJumpPressed;
        public bool isInteractPressed;

        public bool isFire1Pressed;
        public bool isFire2Pressed;

        void Update()
        {
            Control();
        }

        void Control()
        {
            var moveVertical = Input.GetAxis("Vertical");
            var moveHorizontal = Input.GetAxis("Horizontal");
            
            var lookVertical = Input.GetAxis("Mouse Y");
            var lookHorizontal = Input.GetAxis("Mouse X");
            isJumpPressed = Input.GetAxis("Jump") > 0.5f;

            isCrouchPressed = Input.GetAxis("Crouch") > 0.5f;

            isInteractPressed = Input.GetAxis("Interact") > 0.5f;

            isFire1Pressed = Input.GetAxis("Fire1") > 0.5f;

            // is both mouse buttons pressed, then we will count this as Fire1
            isFire2Pressed = Input.GetAxis("Fire2") > 0.5f && !isFire1Pressed;


            move = new Vector3(moveHorizontal, moveVertical);
            look = new Vector3(lookHorizontal, lookVertical);
        }
    }
}