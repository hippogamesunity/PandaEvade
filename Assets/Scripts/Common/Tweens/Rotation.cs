using UnityEngine;

namespace Assets.Scripts.Common.Tweens
{
    public class Rotation : TweenBase
    {
        public Axis Axis;

        public void Update()
        {
            var angle = Speed * Time.time + Period;

            transform.localRotation = Quaternion.Euler((Axis == Axis.X ? 1 : 0) * angle, (Axis == Axis.Y ? 1 : 0) * angle, (Axis == Axis.Z ? 1 : 0) * angle);
        }
    }
}