
namespace GravityDemo
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public sealed partial class GravitationalBody : MonoBehaviour
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
        [SerializeField] private float mass         = 1;
        [SerializeField] private float initialSpeed = 1;
        [SerializeField] private Color color        = Color.white;
        [SerializeField] private bool  render       = true;

        private bool altered;
        #endregion

        #region PROPERTIES
        public float Mass
        {
            get { return mass; }
            set
            {
                if (mass != value)
                {
                    mass    = value;
                    altered = true;
                }
            }
        }

        public float InitialSpeed
        {
            get { return initialSpeed; }
            set { initialSpeed = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public bool Render
        {
            get { return render; }
            set { render = value; }
        }

        public int Index
        { get; set; }
        #endregion

        #region EVENTS
        public event Action<GravitationalBody> OnAltered   = delegate { };
        public event Action<GravitationalBody> OnDestroyed = delegate { }; 
        #endregion

        #region AWAKE
        private void Awake()
        {
            name = typeof(GravitationalBody).Name;
        }
        #endregion

        #region ON VALIDATE
        private void OnValidate()
        {
            altered = true;
        }
        #endregion

        #region ON DESTROY
        private void OnDestroy()
        {
            OnDestroyed(this);
        }
        #endregion

        #region UPDATE
        private void Update()
        {
            if (transform.localScale.x != transform.localScale.y ||
                transform.localScale.z != transform.localScale.z)
                transform.localScale = Vector3.one * transform.localScale.y;

            if (!Application.isPlaying)
            {
                if (transform.hasChanged)
                {
                    transform.hasChanged = false;
                    altered              = true;
                }

                if (altered)
                {
                    altered = false;
                    OnAltered(this);
                }
            }
        }
        #endregion

        #region ON DRAW GIZMOS
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                if (render)
                {
                    Gizmos.color = Color.white * 0;
                    Gizmos.DrawSphere(transform.localPosition, transform.localScale.y);
                    UnityEditor.Handles.ArrowCap(0, transform.position, transform.rotation, initialSpeed);
                }
            }
        }
        #endif
        #endregion
    }
}