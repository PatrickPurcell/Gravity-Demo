
namespace GravityDemo
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GravitationalBody))]
    public class GravitationalBodyEditor : Editor
    {
        #region ON INSPECTOR GUI
        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
                base.OnInspectorGUI();
        }
        #endregion
    }
}