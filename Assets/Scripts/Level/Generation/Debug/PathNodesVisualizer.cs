﻿using UnityEngine;

namespace Level.Generation.Debug
{
    public class PathNodesVisualizer: MonoBehaviour
    {
        /*public PathGeneratorConfig config;
        [Range(1, 10)]
        public float step = 5.0f;

        private List<Prototype> mustSpawnPrototypes; 
        private List<Prototype> canSpawnPrototypes;

        private Map3<PathNode> map; 

        private void Start()
        {
            AssignPrototypes();
            PathGenerator generator = new PathGenerator(config, mustSpawnPrototypes, canSpawnPrototypes);
            map = generator.Generate();
        }

        private void AssignPrototypes()
        {
            mustSpawnPrototypes = new List<Prototype>();
            canSpawnPrototypes = new List<Prototype>();
            PrototypeInfo[] infos = FindObjectsOfType<PrototypeInfo>();
            foreach (PrototypeInfo info in infos)
            {
                if(info.prototypeType == PrototypeInfo.PrototypeType.MustSpawn)
                    mustSpawnPrototypes.Add(info.GetPrototype());
                else
                    canSpawnPrototypes.Add(info.GetPrototype());
            }
        }
        
        private void OnDrawGizmos()
        {
            
        }*/
    }
}