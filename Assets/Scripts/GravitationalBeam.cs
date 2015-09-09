
namespace GravityDemo
{
    using UnityEngine;

    [ExecuteInEditMode]
    public sealed class GravitationalBeam : MonoBehaviour
    {
        #region STRUCT DATA
        public struct Data
        {
            #region FIELDS
            public Vector4 position;
            public Vector3 velocity;
            public float   mass;
            #endregion

            #region PROPERTIES
            public static int Size
            {
                get
                {
                    return sizeof(float) * 4 +
                           sizeof(float) * 3 +
                           sizeof(float);
                }
            }
            #endregion
        }
        #endregion

        #region FIELDS
        [SerializeField, HideInInspector]
        private GameObject sphere;

        [SerializeField]
        private float initialForce = 1; // meters / second

        [SerializeField]
        private float mass = 1;

        private Vector3 velocity;
        private Vector3 position;

        [SerializeField, HideInInspector]
        private Material material;

        [SerializeField, HideInInspector]
        private int index = -1;

        private ComputeBuffer computeBuffer = null;
        #endregion

        #region PROPERTIES
        public float Mass
        {
            get { return mass; }
            set
            {
                mass = value;
            }
        }

        public Vector3 Velocity
        {
            get { return velocity; }
        }
        #endregion

        #region AWAKE
        private void Awake()
        {
            name = typeof(GravitationalBeam).Name;

            if (sphere == null)
            {
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.parent = transform;
                sphere.hideFlags        = HideFlags.HideInHierarchy;
                sphere.name             = "Model";

                material = Resources.Load<Material>("GravitationalBody");
            }

            velocity = transform.forward * initialForce;
        }
        #endregion

        #region ON RENDER OBJECT
        private void OnWillRenderObject()
        {

        }

        private void OnRenderObject()
        {

        }
        #endregion

        #region ON DRAW GIZMOS
        private void OnDrawGizmos()
        {
            Color gizmosColor = Gizmos.color;
            Gizmos.color = Color.white;

            Gizmos.DrawSphere(transform.position, transform.localScale.y * 0.45f);

            UnityEditor.Handles.ArrowCap(0,
                                         transform.position,
                                         transform.rotation,
                                         initialForce);

            Gizmos.color = gizmosColor;
        }
        #endregion
    }
}