using UnityEngine;

namespace Assets.Scripts.Common.Tweens
{
    public class RotationSpring : TweenBase
    {
        public Axis Axis;
        public float From;
        public float To;

        private float _rotation;

        public void Awake()
        {
            _rotation = 2 * Mathf.Rad2Deg * transform.localRotation.z;
        }

        public void Update()
        {
            var angle = _rotation + From + (To - From) * Sin();

            transform.localRotation = Quaternion.Euler(angle * (Axis == Axis.X ? 1 : 0), angle * (Axis == Axis.Y ? 1 : 0), angle * (Axis == Axis.Z ? 1 : 0));
        }
    }
}