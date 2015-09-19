
#if UNITY_EDITOR

namespace GravityDemo
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GravitationalBodyManager))]
    public class GravitationalBodyManagerEditor : Editor
    {
        #region ON INSPECTOR GUI
        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                base.OnInspectorGUI();

                GravitationalBodyManager manager =
                target as GravitationalBodyManager;

                EditorGUILayout.Space();
                bool guiEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUILayout.IntField("Count", manager.Count);
                GUI.enabled = guiEnabled;
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Add Body"))
                    manager.AddBody();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
        #endregion
    }
}

#endif