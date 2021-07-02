using UnityEditor;
using UnityEngine;

namespace Level.Covers.Editor
{
    [CustomEditor(typeof(CoverManager))]
    public class CoverManagerInspector : UnityEditor.Editor
    {

        
        // художественный фильм "Спиздили"
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CoverManager baker = (CoverManager)target;
            if (GUILayout.Button("Bake"))
            {
                baker.BakeCovers();
            }
        }
    }
}
