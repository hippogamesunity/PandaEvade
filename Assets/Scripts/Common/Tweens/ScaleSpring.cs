using UnityEngine;

namespace Assets.Scripts.Common.Tweens
{
    public class ScaleSpring : TweenBase
    {
        public float From;
        public float To;
        public float Dumping;

        private float _amplitude = 1;
        private Vector3 _scale;

        public void Awake()
        {
            _scale = transform.localScale;
        }

        public void Update()
        {
            _amplitude = Mathf.Max(0, _amplitude - Dumping * Time.deltaTime);

            transform.localScale = _scale * (From + (To - From) * Sin() * _amplitude);
     
            if (_amplitude <= 0)
            {
                enabled = false;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            Reset();
        }

        public void OnDisable()
        {
            transform.localScale = _scale;
        }

        public void Reset()
        {
            _amplitude = 1;
        }
    }
}