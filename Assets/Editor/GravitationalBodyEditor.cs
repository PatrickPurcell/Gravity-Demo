
namespace GravityDemo
{
    using UnityEditor;

    [CanEditMultipleObjects                 ]
    [CustomEditor(typeof(GravitationalBody))]
    public sealed class GravitationalBodyEditor : Editor
    {
    }
}