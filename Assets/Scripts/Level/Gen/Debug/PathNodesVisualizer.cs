using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse3D.PathLayer.PathGeneration;

namespace Level.Gen.Debug
{
    public class PathNodesVisualizer: MonoBehaviour
    {
        [Range(1, 10)]
        public float step = 5.0f;

        public IEnumerable<PathNode> nodes;
        private void OnDrawGizmos()
        {
            
        }
    }
}