
#if UNITY_EDITOR

namespace GravityDemo
{
    using UnityEngine;

    public sealed partial class GravitationalBody
    {
        #region FIELDS
        [SerializeField]
        private float _mass = 1;
        #endregion

        #region ON VALIDATE
        private void OnValidate()
        {
            Mass = _mass; _mass = Mass;
        }
        #endregion

        #region ON DRAW GIZMOS
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                if (render)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(transform.localPosition, transform.localScale.y * 0.45f);
                    UnityEditor.Handles.ArrowCap(0, transform.position, transform.rotation, initialForce);
                }
            }
        }
        #endregion
    }
}

#endif