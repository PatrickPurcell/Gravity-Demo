
#if UNITY_EDITOR

namespace GravityDemo
{
    using UnityEditor;
    using UnityEngine;

    public partial class GravitationalBody
    {
        #region ON DRAW GIZMOS
        private void OnDrawGizmos()
        {
            Color gizmosColor = Gizmos.color;
            Gizmos.color = Color.white;

            Gizmos.DrawSphere(transform.position, transform.localScale.y * 0.45f);

            Handles.ArrowCap(0,
                             transform.position,
                             transform.rotation,
                             initialForce);

            Gizmos.color = gizmosColor;
        }
        #endregion
    }
}

#endif