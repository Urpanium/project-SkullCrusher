using System;
using UnityEngine;

namespace Character
{
    public class CameraZoom : MonoBehaviour
    {
        [Range(1, 179)] public float zoomFieldOfView = 20.0f;

        public float zoomSpeed = 10.0f;

        private Camera camera;
        private float initialFieldOfView;

        private void Start()
        {
            camera = GetComponent<Camera>();
            initialFieldOfView = camera.fieldOfView;
        }

        private void Update()
        {
            float currentFieldOfView = camera.fieldOfView;

            float delta = Time.deltaTime * zoomSpeed;

            // TODO: use input system
            if (Input.GetKey(KeyCode.C))
                delta *= zoomFieldOfView - currentFieldOfView;
            else
                delta *= initialFieldOfView - currentFieldOfView;
            
            camera.fieldOfView += delta;
        }
    }
}