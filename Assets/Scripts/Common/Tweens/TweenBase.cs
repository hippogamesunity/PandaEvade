using UnityEngine;

namespace Assets.Scripts.Common.Tweens
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public abstract class TweenBase : Script
    {
        public float Speed;
        public float Period;
        public bool RandomPeriod;
        public bool RandomSpeed;

        private float _time;

        public virtual void OnEnable()
        {
            _time = Time.time;

            if (RandomPeriod)
            {
                Period = CRandom.GetRandom(360);
            }

            if (RandomSpeed)
            {
                Speed *= CRandom.GetRandom(100) / 100f;
            }
        }

        protected float Sin()
        {
            return (Mathf.Sin(Speed * (Time.time - _time) + Period * Mathf.Deg2Rad) + 1) / 2;
        }
    }
}