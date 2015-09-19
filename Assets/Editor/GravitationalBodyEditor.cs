
#if UNITY_EDITOR

namespace GravityDemo
{
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects                 ]
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

#endif