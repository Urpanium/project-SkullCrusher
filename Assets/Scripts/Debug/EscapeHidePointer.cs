﻿using System;
using UnityEngine;

namespace Debug
{
    public class EscapeHidePointer : MonoBehaviour
    {
        // TODO: implement

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.visible = !Cursor.visible;
                Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}