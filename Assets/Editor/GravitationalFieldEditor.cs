
namespace GravityDemo
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GravitationalField))]
    public sealed class GravitationalFieldEditor : Editor
    {
        #region MENU ITEMS
        [MenuItem("GameObject/3D Object/Grid3D")]
        private static void MenuItem3D(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<GravitationalField>();
            gameObject.name = typeof(GravitationalField).Name;
            GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
            Selection.activeGameObject = gameObject;
        }
        #endregion

        #region ON INSPECTOR GUI
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Body"))
                (target as GravitationalField).AddBody();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Beam"))
                (target as GravitationalField).AddBeam();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        #endregion
    }
}