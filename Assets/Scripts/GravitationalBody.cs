
namespace GravityDemo
{
    using UnityEngine;

    [ExecuteInEditMode]
    public sealed partial class GravitationalBody : MonoBehaviour
    {
        #region STRUCT DATA
        public struct Data
        {
            #region FIELDS
            public float   mass;
            public Vector3 position;
            #endregion

            #region PROPERTIES
            public static int Size
            {
                get { return sizeof(float) +
                             sizeof(float) * 3; }
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
        #endregion

        #region AWAKE
        private void Awake()
        {
            name = typeof(GravitationalBody).Name;

            if (sphere == null)
            {
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.parent = transform;
                sphere.hideFlags        = HideFlags.HideInHierarchy;
                sphere.name             = "Model";
            }

            velocity = transform.forward * initialForce;
        }
        #endregion

        #region UPDATE
        private void Update()
        {
            transform.position += velocity * Time.deltaTime;
        }
        #endregion
    }
}