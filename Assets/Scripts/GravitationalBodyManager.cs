
namespace GravityDemo
{
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public sealed class GravitationalBodyManager : GravitationalObject
    {
        #region FIELDS
        [SerializeField, HideInInspector]
        private List<GravitationalBody> bodies;

        private GravitationalBody.Data[] bodyData;
        private ComputeBuffer            bodyBuffer;

        [SerializeField, HideInInspector] private Mesh     mesh;
        [SerializeField, HideInInspector] private Material material;
        #endregion

        #region PROPERTIES
        public int Count
        { get { return bodies.Count; } }

        public ComputeBuffer BodyBuffer
        { get { return bodyBuffer; } }
        #endregion

        #region AWAKE
        private void Awake()
        {
            name = typeof(GravitationalBodyManager).Name;

            if (bodies == null)
                bodies = new List<GravitationalBody>();

            LoadResource<Mesh    >("sphere",            ref mesh);
            LoadResource<Material>("GravitationalBody", ref material);
        }
        #endregion

        #region ON VALIDATE
        private void OnValidate()
        {
            UpdateBodyBuffer();
        }
        #endregion

        #region ON ENABLE / DISABLE
        private void OnEnable()
        {
            foreach (GravitationalBody body in bodies)
                SubscribeToBodyEvents(body);

            UpdateBodyBuffer();
        }

        private void OnDisable()
        {
            ReleaseComputeBuffer(ref bodyBuffer);
        }
        #endregion

        #region UPDATE
        private void Update()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale    = Vector3.one;
        }
        #endregion

        #region ON RENDER OBJECT
        private void OnRenderObject()
        {
            for (int i = 0; i < bodies.Count; ++i)
            {
                material.SetPass(0);
                material.SetInt("index", i);
                material.SetBuffer("body_buffer", bodyBuffer);
                Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
            }
        }
        #endregion

        #region METHODS
        public void AddBody()
        {
            GravitationalBody body =
            new GameObject().AddComponent<GravitationalBody>();
            body.transform.parent = transform;
            bodies.Add(body);

            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = body.gameObject;
            #endif

            SubscribeToBodyEvents(body);

            UpdateBodyBuffer();
        }

        public void RemoveBody(GravitationalBody body)
        {
            bodies.Remove(body);

            UnsubscribeFromBodyEvents(body);

            UpdateBodyBuffer();
        }

        private void OnBodyAltered(GravitationalBody body)
        {
            UpdateBodyBuffer();
        }

        private void UpdateBodyBuffer()
        {
            if (bodies.Count > 0)
            {
                ValidateComputeBuffer(bodies.Count, GravitationalBody.Data.Size, ref bodyBuffer);

                if (bodyData == null || bodyData.Length != bodies.Count)
                    bodyData = new GravitationalBody.Data[bodies.Count];

                for (int i = 0; i < bodies.Count; ++i)
                {
                    bodyData[i].position   = bodies[i].transform.position;
                    bodyData[i].position.w = 1;
                    bodyData[i].velocity   = bodies[i].transform.forward * bodies[i].initialForce;
                    bodyData[i].mass       = bodies[i].Mass;
                }

                bodyBuffer.SetData(bodyData);
            }
        }

        private void SubscribeToBodyEvents(GravitationalBody body)
        {
            UnsubscribeFromBodyEvents(body);

            body.OnDestroyed += RemoveBody;
            body.OnAltered   += OnBodyAltered;
        }

        private void UnsubscribeFromBodyEvents(GravitationalBody body)
        {
            body.OnAltered   -= OnBodyAltered;
            body.OnDestroyed -= RemoveBody;
        }
        #endregion
    }
}