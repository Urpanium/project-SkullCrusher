using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level.Covers
{
    public class CoverManager : MonoBehaviour
    {
        public LayerMask shootableMask;

        public string coverSetName = "HumanCoverSet";
        [Header("Character Settings")] public float characterStandHeight;
        public float characterCrouchHeight;
        public float characterRadius;
        
        [SerializeField]
        private List<Cover> covers;


        private void Awake()
        {
            LoadCovers();
        }

        public void BakeCovers()
        {
            if (characterRadius < 0.01f)
                throw new Exception("Too low character radius");
            CoverBaker baker = new CoverBaker(shootableMask, characterStandHeight, characterCrouchHeight,
                characterRadius);
            covers = baker.BakeCovers();
            SaveCovers();
        }

        private void SaveCovers()
        {
            string sceneResourcesFolder = GetCurrentSceneResourcesFolder();
            
            if (!Directory.Exists(sceneResourcesFolder))
                Directory.CreateDirectory(new DirectoryInfo(sceneResourcesFolder).ToString());
            
            string path = sceneResourcesFolder +
                          Path.DirectorySeparatorChar + coverSetName + ".json";

            File.WriteAllText(path, JsonUtility.ToJson(new SerializableCoverList(covers), true));
        }


        private void LoadCovers()
        {
            string sceneResourcesFolder = GetCurrentSceneResourcesFolder();

            if (!Directory.Exists(sceneResourcesFolder))
                return;
            string path = sceneResourcesFolder +
                          Path.DirectorySeparatorChar + coverSetName + ".json";
            
            if(!File.Exists(path))
                return;
            string content = File.ReadAllText(path);
            SerializableCoverList list = JsonUtility.FromJson<SerializableCoverList>(content);
            covers = list.covers;
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


        private void OnDrawGizmosSelected()
        {
            // visualize covers
            if (covers == null)
                return;
            for (int i = 0; i < covers.Count; i++)
            {
                Cover cover = covers[i];
                Vector3 offset = new Vector3(0, characterStandHeight, 0);
                Gizmos.color = Color.green;
                if (cover.type == CoverType.Crouch)
                {
                    offset = new Vector3(0, characterCrouchHeight, 0);
                    Gizmos.color = Color.yellow;
                }

                Gizmos.DrawSphere(cover.position + offset, 0.125f);
                Gizmos.DrawLine(cover.position + offset, cover.position);
                Gizmos.DrawLine(cover.position + offset, cover.position + offset + cover.direction);
            }
        }
    }
}