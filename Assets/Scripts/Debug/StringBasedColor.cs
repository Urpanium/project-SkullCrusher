using UnityEngine;
using Util;

namespace Debug
{
    public class StringBasedColor: MonoBehaviour
    {
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = ColorUtil.GetStringBasedColor(name);
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
    }
}