
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

        [SerializeField] private bool randomize = false;

        private bool updateBuffer;
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
            UpdateBuffer(true);
        }
        #endregion

        #region ON ENABLE / DISABLE
        private void OnEnable()
        {
            foreach (GravitationalBody body in bodies)
                SubscribeToBodyEvents(body);

            UpdateBuffer(true);
        }

        private void OnDisable()
        {
            ReleaseComputeBuffer(ref buffer);
        }
        #endregion

        #region UPDATE
        protected override void Update()
        {
            base.Update();

            UpdateBuffer();
        }
        #endregion

        #region ON RENDER OBJECT
        private void OnRenderObject()
        {
            for (int i = 0; i < bodies.Count; ++i)
                if (bodies[i].Render)
                {
                    GravitationalBody body = bodies[i];

                    material.SetPass(0);
                    material.SetInt   ("index",       i);
                    material.SetFloat ("scale",       body.transform.localScale.y);
                    material.SetColor ("color",       body.Color);
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

            if (randomize)
            {
                body.Mass         = Random.Range(1, 500);
                body.InitialSpeed = Random.Range(0, 4);
                body.transform.localEulerAngles =
                new Vector3(0, Random.Range(0, 360), 0);
                
                GravitationalField gravitationalField =
                transform.parent.GetComponent<GravitationalField>();
                float w = gravitationalField.Width  * 0.5f;
                float h = gravitationalField.Height * 0.5f;
                float d = gravitationalField.Depth  * 0.5f;
                float x = Random.Range(-w, w);
                float z = Random.Range(-d, d);
                float r = Random.Range(0.0f, 1.0f);
                float g = Random.Range(0.0f, 1.0f);
                float b = Random.Range(0.0f, 1.0f);
                body.transform.localPosition = new Vector3(x, 0, z);
                body.Color                   = new Color(r, g, b, 1);
            }
            else
            {
                #if UNITY_EDITOR
                UnityEditor.Selection.activeGameObject = body.gameObject;
                #endif
            }

            SubscribeToBodyEvents(body);
            UpdateBuffer(true);
        }

        public void RemoveBody(GravitationalBody body)
        {
            UnsubscribeFromBodyEvents(body);
            bodies.Remove(body);
            updateBuffer = true;
        }

        private void OnBodyAltered(GravitationalBody body)
        {
            UpdateBuffer(true);
        }

        private void UpdateBuffer(bool forceUpdate = false)
        {
            if (updateBuffer || forceUpdate)
            {
                updateBuffer = false;

                if (bodies.Count > 0)
                {
                    ValidateComputeBuffer(bodies.Count, GravitationalBody.Data.Size, ref buffer);

                    if (data == null || data.Length != bodies.Count)
                        data = new GravitationalBody.Data[bodies.Count];

                    for (int i = 0; i < bodies.Count; ++i)
                    {
                        GravitationalBody body = bodies[i];

                        Vector3 position = body.transform.position;
                        if (position.x % 1 == 0 &&
                            position.y % 1 == 0 &&
                            position.z % 1 == 0)
                            position[Random.Range(0, 3)] += 0.0001f;

                        data[i].position   = position;
                        data[i].position.w = 1;
                        data[i].velocity   = body.transform.forward * body.InitialSpeed;
                        data[i].mass       = body.Mass;
                    }

                    buffer.SetData(data);
                }
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