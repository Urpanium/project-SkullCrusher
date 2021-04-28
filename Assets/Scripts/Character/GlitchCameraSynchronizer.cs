using System;
using UnityEngine;

namespace Character
{
    public class GlitchCameraSynchronizer : MonoBehaviour
    {
        public Camera playerCamera;

        private Camera camera;

        private void Start()
        {
            camera = GetComponent<Camera>();
        }
        private void Update()
        {
            camera.fieldOfView = playerCamera.fieldOfView;
        }
    }
}