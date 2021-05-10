﻿using UnityEngine;

namespace PostProcessing
{
    [ExecuteInEditMode]
    public class ImageEffect : MonoBehaviour
    {
        public Material material;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }
    }
}