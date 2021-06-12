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


        private List<Dector3> currentEntries;
        private List<Dector3> allEntries;

        private void Start()
        {
           
            generator = new PathGenerator(config, mustSpawnPrototypes, canSpawnPrototypes);
            generator.Reset();
            map = generator.GenStep();
            currentEntries = generator.GetCurrentEntries();
            allEntries = generator.GetAllEntries();
            PathTile tile = new PathTile();
            tile.SetDirectionAccess(0,true);
            tile.SetDirectionAccess(3,true);
            tile.Changeable = false;
            print(tile);
            tile = PathTile.FromByte(tile.ToByte());
            print(tile);
            Dector3.DirCheck();
        }

        public void Step()
        {
            map = generator.GenStep();
            currentEntries = generator.GetCurrentEntries();
            allEntries = generator.GetAllEntries();
            
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

        private void OnDrawGizmosSelected()
        {
            if (map == null)
                return;
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
                                
                                if (currentEntries.Contains(dPosition))
                                {
                                    Gizmos.color = Color.green;
                                }
                                else
                                {
                                    if (allEntries.Contains(dPosition))
                                    {
                                        Gizmos.color = Color.blue;
                                    }
                                    else
                                    {
                                        int connections = 0;
                                        for (int i = 0; i < Dector3.Directions.Length; i++)
                                        {
                                            if (tile.GetDirectionAccess(i))
                                            {
                                                connections++;
                                            }
                                        }

                                        Gizmos.color = new Color(
                                            (connections - 1) / 5.0f,
                                            0,
                                            0
                                        );
                                    }
                                }

                                
                                
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