
namespace GravityDemo
{
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public sealed partial class GravitationalField : MonoBehaviour
    {
        #region FIELDS
        [SerializeField, HideInInspector]
        private List<GravitationalBody> bodies = new List<GravitationalBody>();

        [SerializeField, HideInInspector] private int width  = 8;
        [SerializeField, HideInInspector] private int height = 8;
        [SerializeField, HideInInspector] private int depth  = 8;

        [SerializeField, HideInInspector] private ComputeShader gravitationalField;
        [SerializeField, HideInInspector] private ComputeShader gravitationalFieldVelocity;
        [SerializeField, HideInInspector] private Material      pointsMaterial;
        [SerializeField, HideInInspector] private Material      gridMaterial;

        [SerializeField] private bool drawPoints = true;
        [SerializeField] private bool drawGrid   = true;

        //private GravitationalBody.Data[] bodyData;
        private Vector4[] bodyData;

        private ComputeBuffer pointBuffer;
        private ComputeBuffer gridBuffer;
        private ComputeBuffer bodyBuffer;

        private int computePointPositionsKernel;
        private int computeDisplacementKernel;
        private int computeGridKernel;
        private int computeVelocityKernel;
        #endregion

        #region PROPERTIES
        public int Width
        {
            get { return width; }
            set { width = Mathf.Max(1, value); }
        }

        public int Height
        {
            get { return height; }
            set { height = Mathf.Max(1, value); }
        }

        public int Depth
        {
            get { return depth; }
            set { depth = Mathf.Max(1, value); }
        }

        private int W        { get { return width  + 1; } }
        private int H        { get { return height + 1; } }
        private int D        { get { return depth  + 1; } }
        private int ThreadsX { get { return W;          } }
        private int ThreadsY { get { return H;          } }
        private int ThreadsZ { get { return D;          } }

        private int PointCount
        {
            get
            {
                return (width  + 1) *
                       (height + 1) *
                       (depth  + 1);
            }
        }
        #endregion

        #region AWAKE
        private void Awake()
        {
            #if UNITY_EDITOR
            OnValidate();
            #endif
        }
        #endregion

        #region ON ENABLE / DISABLE
        private void OnEnable()
        {
            LoadResource("GravitationalField",         ref gravitationalField);
            LoadResource("GravitationalFieldVeloctiy", ref gravitationalFieldVelocity);
            LoadResource("GravitationalFieldPoints",   ref pointsMaterial);
            LoadResource("GravitationalFieldGrid",     ref gridMaterial);

            computePointPositionsKernel = gravitationalField.FindKernel("ComputePointPositions");
            computeDisplacementKernel   = gravitationalField.FindKernel("ComputeDisplacement");
            computeGridKernel           = gravitationalField.FindKernel("ComputeGrid");

            computeVelocityKernel = gravitationalFieldVelocity.FindKernel("ComputeVelocity");
        }

        private void OnDisable()
        {
            ReleaseComputeBuffer(ref pointBuffer);
            ReleaseComputeBuffer(ref gridBuffer);
            ReleaseComputeBuffer(ref bodyBuffer);
        }
        #endregion

        #region ON DESTROY
        private void OnDestroy()
        {
            Resources.UnloadAsset(pointsMaterial);
            Resources.UnloadAsset(gridMaterial);
        }
        #endregion

        #region ON RENDER OBJECT
        private void OnRenderObject()
        {
            ValidatePointBuffer();
            ValidateGridBuffer();
            ValidateBodyBuffer();

            if (bodies.Count > 0)
            {
                for (int i = 0; i < bodies.Count; ++i)
                {
                    //bodyData[i].mass     = 1;
                    //bodyData[i].position = bodies[i].transform.position;
                    bodyData[i]   = bodies[i].transform.position;
                    bodyData[i].w = bodies[i].Mass;
                }

                bodyBuffer.SetData(bodyData);
                //computeShader.Dispatch(computeDisplacementKernel, ThreadsX, ThreadsY, ThreadsZ);
            }

            gravitationalField.SetInt("body_count", bodies.Count);
            gravitationalField.Dispatch(computeDisplacementKernel, ThreadsX, ThreadsY, ThreadsZ);

            if (drawPoints)
                DrawField(pointsMaterial);

            if (drawGrid)
            {
                gravitationalField.Dispatch(computeGridKernel, ThreadsX, ThreadsY, ThreadsZ);
                DrawField(gridMaterial);
            }

            gravitationalFieldVelocity.Dispatch(computeVelocityKernel, 1, 1, 1);
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
        }

        private void DrawField(Material material)
        {
            material.SetPass(0);
            material.SetMatrix("object_to_world", transform.localToWorldMatrix);
            Graphics.DrawProcedural(MeshTopology.Points, PointCount);
        }

        private void ValidatePointBuffer()
        {
            if (ValidateComputeBuffer(PointCount, sizeof(float) * 3 * 2, ref pointBuffer))
            {
                gravitationalField.SetInt   ("w", W);
                gravitationalField.SetInt   ("h", H);
                gravitationalField.SetInt   ("d", D);
                gravitationalField.SetVector("offset", new Vector3(width, height, depth) * 0.5f);
                gravitationalField.SetBuffer(computePointPositionsKernel, "point_buffer", pointBuffer);
                gravitationalField.Dispatch(computePointPositionsKernel, ThreadsX, ThreadsY, ThreadsZ);

                pointsMaterial.SetBuffer("point_buffer", pointBuffer);
            }
        }

        private void ValidateGridBuffer()
        {
            if (ValidateComputeBuffer(PointCount, sizeof(uint) * 3, ref gridBuffer))
            {
                gravitationalField.SetBuffer(computeGridKernel, "point_buffer", pointBuffer);
                gravitationalField.SetBuffer(computeGridKernel, "grid_buffer",  gridBuffer);
                gridMaterial      .SetBuffer(                   "point_buffer", pointBuffer);
                gridMaterial      .SetBuffer(                   "grid_buffer",  gridBuffer);
            }
        }

        private void ValidateBodyBuffer()
        {
            if (bodies.Count > 0)
            {
                if (ValidateComputeBuffer(bodies.Count, sizeof(float) * 4, ref bodyBuffer))
                {
                    gravitationalField.SetInt("body_count", bodies.Count);
                    gravitationalField.SetBuffer(computeDisplacementKernel, "point_buffer", pointBuffer);
                    gravitationalField.SetBuffer(computeDisplacementKernel, "body_buffer",  bodyBuffer);
                }

                if (bodyData == null || bodyData.Length != bodies.Count)
                {
                    //bodyData = new GravitationalBody.Data[bodies.Count];
                    bodyData = new Vector4[bodies.Count];
                }
            }
        }

        private void LoadResource<T>(string resourcePath, ref T resource) where T : UnityEngine.Object
        {
            if (resource == null)
                resource = Resources.Load<T>(resourcePath);
        }

        private bool ValidateComputeBuffer(int count, int stride, ref ComputeBuffer computeBuffer)
        {
            bool computeBufferAllocated = false;
            if (computeBuffer == null || computeBuffer.count != count || computeBuffer.stride != stride)
            {
                ReleaseComputeBuffer(ref computeBuffer);
                computeBuffer = new ComputeBuffer(count, stride);
                computeBufferAllocated = true;
            }

            return computeBufferAllocated;
        }

        private void ReleaseComputeBuffer(ref ComputeBuffer computeBuffer)
        {
            if (computeBuffer != null)
            {
                computeBuffer.Release();
                computeBuffer.Dispose();
                computeBuffer = null;
            }
        }
        #endregion

        #region ON DRAW GIZMOS
        private void OnDrawGizmos()
        {
            Color     gizmosColor  = Gizmos.color;
            Matrix4x4 gizmosMatrix = Gizmos.matrix;

            Gizmos.color  = new Color(1, 1, 1, 0.25f);
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, depth));

            Gizmos.matrix = gizmosMatrix;
            Gizmos.color  = gizmosColor;
        }
        #endregion
    }
}