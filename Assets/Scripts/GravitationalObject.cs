
namespace GravityDemo
{
    using UnityEngine;

    public abstract class GravitationalObject : MonoBehaviour
    {
        #region METHODS
        protected void LoadResource<T>(string resourcePath, ref T resource) where T : Object
        {
            if (resource == null)
                resource = Resources.Load<T>(resourcePath);
        }

        protected bool ValidateComputeBuffer(int count, int stride, ref ComputeBuffer computeBuffer)
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

        protected void ReleaseComputeBuffer(ref ComputeBuffer computeBuffer)
        {
            if (computeBuffer != null)
            {
                computeBuffer.Release();
                computeBuffer.Dispose();
                computeBuffer = null;
            }
        }
        #endregion
    }
}