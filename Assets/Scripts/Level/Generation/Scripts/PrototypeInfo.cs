using System;
using System.Collections.Generic;
using Level.Generation.PathLayer.Path.Prototypes;
using Level.Generation.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace Level.Generation.Scripts
{
    public class PrototypeInfo: MonoBehaviour
    {

        public Dector3 size;
        public List<string> sockets;
        public List<Dector3> socketsPositions;

        private void Awake()
        {
            if (sockets.Count != socketsPositions.Count)
            {
                UnityEngine.Debug.LogError("Amount of sockets and their positions are not same!", this);
            }
        }

        public PathPrototype ToPathPrototype()
        {
            PathPrototype pathPrototype = new PathPrototype(size, socketsPositions);
            return pathPrototype;
        }
        /*
         * ToGameplayPrototype()
         * 
         * ToVisualPrototype()
         */

        private void OnDrawGizmos()
        {
            if (sockets.Count != socketsPositions.Count)
            {
                UnityEngine.Debug.LogError("Amount of sockets and their positions are not same!", this);
                return;
            }

            for (int i = 0; i < sockets.Count; i++)
            {
                Dector3 localPosition = socketsPositions[i];
                Vector3 worldPosition = transform.position + localPosition;
                
                Handles.Label(worldPosition, sockets[i]);
            }
        }
    }
}