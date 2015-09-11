
#if UNITY_EDITOR

namespace GravityDemo
{
    using UnityEngine;

    public partial class GravitationalField
    {
        #if UNITY_EDITOR
        [SerializeField] private int _width  = 8;
        [SerializeField] private int _height = 8;
        [SerializeField] private int _depth  = 8;
        #endif

        #region ON VALIDATE
        private void OnValidate()
        {
            Width  = _width;  _width  = Width;
            Height = _height; _height = Height;
            Depth  = _depth;  _depth  = Depth;
        }
        #endregion
    }
}

#endif