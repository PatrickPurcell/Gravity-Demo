
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

        private GravitationalBody.Data[] data;
        private ComputeBuffer            buffer;

        [SerializeField, HideInInspector] private Mesh     mesh;
        [SerializeField, HideInInspector] private Material material;
        #endregion

        #region PROPERTIES
        public int Count
        { get { return bodies.Count; } }

        public ComputeBuffer Buffer
        { get { return buffer; } }
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
            UpdateBuffers();
        }
        #endregion

        #region ON ENABLE / DISABLE
        private void OnEnable()
        {
            foreach (GravitationalBody body in bodies)
                SubscribeToBodyEvents(body);

            UpdateBuffers();
        }

        private void OnDisable()
        {
            ReleaseComputeBuffer(ref buffer);
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
                if (bodies[i].Render)
                {
                    material.SetPass(0);
                    material.SetInt("index", i);
                    material.SetFloat("scale", bodies[i].transform.localScale.y);
                    material.SetBuffer("body_buffer", buffer);
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

            body.Mass                    = Random.Range(1, 500);
            body.InitialSpeed            = Random.Range(0, 4);
            body.transform.localRotation = Random.rotationUniform;

            GravitationalField gravitationalField =
            transform.parent.GetComponent<GravitationalField>();
            float w = gravitationalField.Width  * 0.5f;
            float h = gravitationalField.Height * 0.5f;
            float d = gravitationalField.Depth  * 0.5f;
            float x = Random.Range(-w, w);
            float y = Random.Range(-h, h);
            float z = Random.Range(-d, d);
            body.transform.localPosition = new Vector3(x, y, z);

            //#if UNITY_EDITOR
            //UnityEditor.Selection.activeGameObject = body.gameObject;
            //#endif

            SubscribeToBodyEvents(body);

            UpdateBuffers();
        }

        public void RemoveBody(GravitationalBody body)
        {
            bodies.Remove(body);

            UnsubscribeFromBodyEvents(body);

            UpdateBuffers();
        }

        private void OnBodyAltered(GravitationalBody body)
        {
            UpdateBuffers();
        }

        private void UpdateBuffers()
        {
            if (bodies.Count > 0)
            {
                ValidateComputeBuffer(bodies.Count, GravitationalBody.Data.Size, ref buffer);

                if (data == null || data.Length != bodies.Count)
                    data = new GravitationalBody.Data[bodies.Count];

                for (int i = 0; i < bodies.Count; ++i)
                {
                    data[i].position   = bodies[i].transform.position;
                    data[i].position.w = 1;
                    data[i].velocity   = bodies[i].transform.forward * bodies[i].InitialSpeed;
                    data[i].mass       = bodies[i].Mass;
                }

                buffer.SetData(data);
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