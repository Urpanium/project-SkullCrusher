using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Level.Covers.Classes;
using Level.Covers.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using Color = System.Drawing.Color;

namespace Level.Covers
{
    [ExecuteInEditMode]
    // менеджер ковров
    public class CoverManager : MonoBehaviour
    {
        public LayerMask shootableMask;
        public string coverSetName = "HumanCoverSet";

        [Header("Character Settings")] public float characterStandHeight;
        public float characterCrouchHeight;
        public float characterRadius;

        [Header("Clusterization Settings")] public int clustersCount = 5;
        public bool showClustersVolumes = false;
        [Header("Accuracy Settings")] public float linePointsEpsilon = 0.01f;
        

        private CoverBaker baker;
        private CoverClusterMaker clusterMaker;

        private List<Vector3> centers;
        private List<List<Cover>> efficientCoverList;

        // for visualization
        private List<(Vector3, Vector3)> bounds;
        private List<Cover> covers;
        private List<int> clusterIds;


        private void Awake()
        {
            SerializableCoverList loadedCovers = LoadCovers();
            covers = loadedCovers.covers;
            clusterIds = loadedCovers.clusterIds;
            if (covers.Count > 0 && clusterIds.Count > 0)
            {
                efficientCoverList = MakeEfficientCoversList(covers, clusterIds);
            }
        }

        public void BakeCovers()
        {
            if (characterRadius < 0.01f)
                throw new Exception("Too low character radius");

            baker = new CoverBaker(shootableMask, characterStandHeight, characterCrouchHeight,
                characterRadius, linePointsEpsilon);
            List<Cover> covers = baker.BakeCovers();

            clusterMaker = new CoverClusterMaker(clustersCount, covers);
            List<int> clusterIds = clusterMaker.Clusterize(out centers);

            SerializableCoverList serializableCoverList = new SerializableCoverList(covers, clusterIds);
            SaveCovers(serializableCoverList);
            efficientCoverList = MakeEfficientCoversList(covers, clusterIds);

            bounds = new List<(Vector3, Vector3)>();
            for (int i = 0; i < clustersCount; i++)
            {
                bounds.Add(GetClusterBounds(i));
            }
        }

        private void SaveCovers(SerializableCoverList serializableCoverList)
        {
            string sceneResourcesFolder = GetCurrentSceneResourcesFolder();

            if (!Directory.Exists(sceneResourcesFolder))
                Directory.CreateDirectory(new DirectoryInfo(sceneResourcesFolder).ToString());

            string path = sceneResourcesFolder +
                          Path.DirectorySeparatorChar + coverSetName + ".json";

            File.WriteAllText(path, JsonUtility.ToJson(serializableCoverList, false));
        }

        private (Vector3, Vector3) GetClusterBounds(int clusterId)
        {
            List<Cover> cluster = efficientCoverList[clusterId];
            
            Vector3 minBound = cluster[0].position;
            Vector3 maxBound = cluster[0].position;
            
            for (int i = 1; i < cluster.Count; i++)
            {
                Vector3 position = cluster[i].position;
                
                if (maxBound.x > position.x)
                    maxBound.x = position.x;
                if (maxBound.y > position.y)
                    maxBound.y = position.y;
                if (maxBound.z > position.z)
                    maxBound.z = position.z;

                if (minBound.x < position.x)
                    minBound.x = position.x;
                if (minBound.y < position.y)
                    minBound.y = position.y;
                if (minBound.z < position.z)
                    minBound.z = position.z;
            }

            return (minBound, maxBound);
        }


        private SerializableCoverList LoadCovers()
        {
            string sceneResourcesFolder = GetCurrentSceneResourcesFolder();

            if (!Directory.Exists(sceneResourcesFolder))
                return null;
            string path = sceneResourcesFolder +
                          Path.DirectorySeparatorChar + coverSetName + ".json";

            if (!File.Exists(path))
                return null;
            string content = File.ReadAllText(path);
            SerializableCoverList list = JsonUtility.FromJson<SerializableCoverList>(content);
            return list;
        }

        private string GetCurrentSceneResourcesFolder()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            FileInfo sceneFileInfo = new FileInfo(currentScene.path);
            string sceneResourcesFolder =
                sceneFileInfo.Directory + Path.DirectorySeparatorChar.ToString() + currentScene.name +
                Path.DirectorySeparatorChar + "Resources";
            return sceneResourcesFolder;
        }

        private List<List<Cover>> MakeEfficientCoversList(List<Cover> covers, List<int> clusterIds)
        {
            List<List<Cover>> result = new List<List<Cover>>();
            for (int i = 0; i < clustersCount; i++)
            {
                result.Add(new List<Cover>());
            }

            for (int i = 0; i < covers.Count; i++)
            {
                Cover cover = covers[i];
                int clusterId = clusterIds[i];

                result[clusterId].Add(cover);
            }

            return result;
        }

        private void OnDrawGizmosSelected()
        {
            // visualize covers
            if (covers != null)
            {
                for (int i = 0; i < covers.Count; i++)
                {
                    Gizmos.color = ColorUtil.GetStringBasedColor(clusterIds[i].ToString());
                    Cover cover = covers[i];
                    Vector3 offset = new Vector3(0, characterStandHeight, 0);

                    if (cover.type == CoverType.Crouch)
                    {
                        offset = new Vector3(0, characterCrouchHeight, 0);
                    }

                    Gizmos.DrawSphere(cover.position + offset, 0.125f);
                    Gizmos.DrawLine(cover.position + offset, cover.position);
                    Gizmos.DrawLine(cover.position + offset, cover.position + offset + cover.direction);
                }
            }

            if (bounds != null && showClustersVolumes)
            {
                for (int i = 0; i < bounds.Count; i++)
                {
                    Gizmos.color = ColorUtil.GetStringBasedColor(i.ToString());
                    (Vector3, Vector3) bound = bounds[i];
                    Vector3 center = Vector3.Lerp(bound.Item1, bound.Item2, 0.5f);
                    Vector3 delta = bound.Item1 - bound.Item2;
                    delta.x = Mathf.Abs(delta.x);
                    delta.y = Mathf.Abs(delta.y);
                    delta.z = Mathf.Abs(delta.z);
                    
                    Gizmos.DrawWireCube(center, delta);
                }
            }
        }
    }
}