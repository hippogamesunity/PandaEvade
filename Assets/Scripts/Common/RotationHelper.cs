using UnityEngine;

namespace Assets.Scripts.Common
{
    public static class RotationHelper
    {
        public static float Angle(Vector2 direction)
        {
            if (direction == Vector2.zero) return 0;

            return Vector3.Angle(Vector3.up, direction) * -Mathf.Sign(direction.x);
        }
    }
}