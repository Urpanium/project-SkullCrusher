using System;
using UnityEngine;

namespace Level.Single_Instance
{
    public class InputManager : MonoBehaviour
    {
        /*public enum Interpolation
        {
            Linear = 0,
            Quintic,
            Cosine,
            Cubic,
            Strange
        }

       

        [Header("Look Settings")] public bool lookInterpolation = true;
        public Interpolation interpolation;
        public float interpolationSpeed = 1.0f;
        public int strangePower = 3;
        [Range(0.01f, 1.0f)] public float lookSensitivity = 0.5f;
        public bool invertY;
        public bool invertX;

        [Header("Read-only")] public Vector3 move;
        public float moveInputSpeedMultiplier;
        public Vector3 look;

        public bool isJumpPressed;


        void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            actions = playerInput.actions;
            moveAction = actions["Move"];
            lookAction = actions["Look"];
            jumpAction = actions["Jump"];
            interactAction = actions["Interact"];
        }


        private void Update()
        {
            move = moveAction.ReadValue<Vector2>();
            if (lookInterpolation)
                look = InterpolateLook(lookAction.ReadValue<Vector2>() * lookSensitivity);
            else
                look = lookAction.ReadValue<Vector2>() * lookSensitivity;
            if (invertY)
                look.y *= -1;
            if (invertX)
                look.x *= -1;
            isJumpPressed = jumpAction.ReadValue<float>() > 0;
        }

        private Vector2 InterpolateLook(Vector2 input)
        {
            float v = Time.deltaTime * interpolationSpeed;
            if (input.x == 0 && input.y == 0)
                return new Vector2();
            switch (interpolation)
            {
                case Interpolation.Linear:
                    return Vector2.Lerp(look, input, v);
                case Interpolation.Quintic:
                    return Vector2.Lerp(look, input, QuinticCurve(v));
                case Interpolation.Cosine:
                    return Vector2.Lerp(look, input, CosineCurve(v));
                case Interpolation.Cubic:
                    return Vector2.Lerp(look, input, CubicCurve(v));
                default:
                    return Vector2.Lerp(look, input, StrangeCurve(v));
            }
        }

        private float QuinticCurve(float t)
        {
            return t * t * t * (10 + t * (t * 6 - 15));
        }

        private float CosineCurve(float t)
        {
            return (float) (1 - Math.Cos(t * Math.PI) * 0.5);
        }

        private float CubicCurve(float t)
        {
            return -t * t * (2 * t - 3);
        }

        private float StrangeCurve(float t)
        {
            float initial = (float) Math.Sin(Math.PI * t - Math.PI * 0.5f) * 0.5f + 0.5f;
            return (float) Math.Pow(initial, strangePower);
        }


        public void OnDeviceLost()
        {
            
        }

        public void OnDeviceRegained()
        {
        }

        public void OnControlsChanged()
        {
        }*/
    }
}