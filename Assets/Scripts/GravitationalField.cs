
namespace GravityDemo
{
    using UnityEngine;

    [ExecuteInEditMode]
    public sealed partial class GravitationalField : GravitationalObject
    {
        #region FIELDS
        [SerializeField, HideInInspector]
        private GravitationalBodyManager bodies;

        [SerializeField, HideInInspector] private int width  = 8;
        [SerializeField, HideInInspector] private int height = 8;
        [SerializeField, HideInInspector] private int depth  = 8;

        [SerializeField, HideInInspector] private ComputeShader gravitationalField;
        [SerializeField, HideInInspector] private ComputeShader gravitationalFieldVelocity;
        [SerializeField, HideInInspector] private Material      pointsMaterial;
        [SerializeField, HideInInspector] private Material      gridMaterial;

        [SerializeField] private bool drawPoints = true;
        [SerializeField] private bool drawGrid   = true;

        private ComputeBuffer pointBuffer;
        private ComputeBuffer gridBuffer;

        private int computePointPositionsKernel;
        private int computeDisplacementKernel;
        private int computeGridKernel;
        private int computeVelocityKernel;
        #endregion

        #region PROPERTIES
        public int Width  { get { return width;  } set { width  = Mathf.Max(1, value); } }
        public int Height { get { return height; } set { height = Mathf.Max(1, value); } }
        public int Depth  { get { return depth;  } set { depth  = Mathf.Max(1, value); } }

        private int W          { get { return width  + 1; } }
        private int H          { get { return height + 1; } }
        private int D          { get { return depth  + 1; } }
        private int ThreadsX   { get { return W;          } }
        private int ThreadsY   { get { return H;          } }
        private int ThreadsZ   { get { return D;          } }
        private int PointCount { get { return W * H * D;  } }
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
            if (bodies == null)
            {
                bodies =
                new GameObject().AddComponent<GravitationalBodyManager>();
                bodies.transform.parent = transform;
            }

            LoadResource("GravitationalField",         ref gravitationalField);
            LoadResource("GravitationalFieldVelocity", ref gravitationalFieldVelocity);
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

            if (bodies.Count > 0)
                gravitationalField.SetBuffer(computeDisplacementKernel, "body_buffer", bodies.Buffer);
            gravitationalField.SetInt("body_count", bodies.Count);
            gravitationalField.SetBuffer(computeDisplacementKernel, "point_buffer", pointBuffer);
            gravitationalField.Dispatch(computeDisplacementKernel, ThreadsX, ThreadsY, ThreadsZ);

            if (drawPoints)
                DrawField(pointsMaterial);

            if (drawGrid)
            {
                gravitationalField.Dispatch(computeGridKernel, ThreadsX, ThreadsY, ThreadsZ);
                DrawField(gridMaterial);
            }

            ComputeVelocity();
        }
        #endregion

        #region METHODS
        private void ComputeVelocity()
        {
            if (Application.isPlaying)
            {
                if (bodies.Count > 0)
                {
                    gravitationalFieldVelocity.SetInt   (                       "w",            W);
                    gravitationalFieldVelocity.SetInt   (                       "h",            H);
                    gravitationalFieldVelocity.SetInt   (                       "d",            D);
                    gravitationalFieldVelocity.SetFloat (                       "delta_time",   Time.deltaTime);
                    gravitationalFieldVelocity.SetBuffer(computeVelocityKernel, "point_buffer", pointBuffer);
                    gravitationalFieldVelocity.SetBuffer(computeVelocityKernel, "body_buffer",  bodies.Buffer);
                    gravitationalFieldVelocity.Dispatch(computeVelocityKernel, bodies.Count, 1, 1);
                }
            }
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

                gridMaterial.SetBuffer("point_buffer", pointBuffer);
                gridMaterial.SetBuffer("grid_buffer",  gridBuffer);
            }
        }
        #endregion

        #region ON DRAW GIZMOS
        private void OnDrawGizmos()
        {
            Gizmos.color  = new Color(1, 1, 1, 0.25f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, depth));
        }
        #endregion
    }
}