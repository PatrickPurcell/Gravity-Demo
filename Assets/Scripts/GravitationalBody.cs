
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
        [SerializeField]
        public float initialForce = 1; // meters / second

        [SerializeField, HideInInspector]
        private float mass = 1;

        private Vector3 velocity;

        private bool altered;

        [SerializeField]
        private bool render = true;
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

            velocity = transform.forward * initialForce;
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

            //transform.position += velocity * Time.deltaTime;
            //
            //if (!Application.isPlaying)
            //{
            //
            //}
            //
            //transform.localScale = Vector3.one * transform.localScale.y;
        }
        #endregion
    }
}