using System;
using System.Collections.Generic;
using Level.Generation.Util;
using UnityEditor;
using UnityEngine;

namespace Level.GenScripts
{
    public class PrototypeInfo : MonoBehaviour
    {
        public enum PrototypeType
        {
            MustSpawn = 0,
            CanSpawn
        }

        public PrototypeType prototypeType;
        public Dector3 size = new Dector3(-1, -1, -1);
        public List<string> sockets;
        public List<Dector3> socketsPositions;
        public float weight = 1;


        private void Start()
        {
            if (size.x < 0)
            {
                // TODO: auto size 
            }
        }

        /*public Prototype GetPrototype()
        {
            Prototype prototype = new Prototype(gameObject.name, sockets, socketsPositions, size);
            return prototype;
        }*/


        private void OnDrawGizmos()
        {
            for (int i = 0; i < Math.Min(socketsPositions.Count, sockets.Count); i++)
            {
                Dector3 position = socketsPositions[i];
                Handles.Label(transform.position + position, sockets[i]);
            }
        }
    }
}