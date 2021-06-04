using System.Collections.Generic;
using Level.Generation.PathLayer.Path;
using Level.Generation.PathLayer.Path.Map;
using Level.Generation.PathLayer.Path.Prototypes;
using Level.Generation.Util;
using UnityEngine;

namespace Level.Generation.Debug
{
    public class PathNodesVisualizer : MonoBehaviour
    {
        public PathGenerationConfig config;
        [Range(1, 10)] public float step = 5.0f;


        public int iterations = 10;
        private List<PathPrototype> mustSpawnPrototypes;
        private List<PathPrototype> canSpawnPrototypes;

        private int genStep = 0;
        private PathMap map;

        private PathGenerator generator;

        private void Start()
        {
           
            generator = new PathGenerator(config, mustSpawnPrototypes, canSpawnPrototypes);

            map = generator.Generate();
            
            //map = generator.Step(genStep++);
        }

        public void Step()
        {
            //map = generator.Step(genStep++);
        }
        

        /*private void AssignPrototypes()
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
        }*/

        private void OnDrawGizmos()
        {
            if (map == null)
                return;
            Color[] colors = {Color.black, Color.blue, Color.cyan, Color.white, Color.yellow, Color.red, Color.magenta};
            Gizmos.color = Color.blue;
            for (int x = map.minBound.x; x <= map.maxBound.x; x++)
            {
                for (int y = map.minBound.y; y <= map.maxBound.y; y++)
                {
                    for (int z = map.minBound.z; z <= map.maxBound.z; z++)
                    {
                        Dector3 dPosition = new Dector3(x, y, z);
                        if (map.IsPositionValid(dPosition))
                        {
                            byte byteTile = map.GetTile(dPosition);
                            if (!map.IsTileEmpty(dPosition))
                            {
                                PathTile tile = PathTile.FromByte(byteTile);
                                int colorIndex = 0;
                                for (int i = 0; i < Dector3.Directions.Length; i++)
                                {
                                    if (tile.GetDirectionAccess(i))
                                    {
                                        colorIndex++;
                                    }
                                }
                                Gizmos.color = colors[colorIndex];
                                
                                Vector3 position = ((Vector3) dPosition - (Vector3)map.size * 0.5f) * step;
                                Gizmos.DrawCube(position, Vector3.one);
                                
                                
                                
                                for (int i = 0; i < Dector3.Directions.Length; i++)
                                {
                                    if (tile.GetDirectionAccess(i))
                                    {
                                        Gizmos.DrawLine(position, position + (Vector3) Dector3.GetDirection(i) * step * 0.333f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}