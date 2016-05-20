using UnityEngine;

namespace Assets.Scripts.Common.Tweens
{
    public class ScaleSpring : TweenBase
    {
        public Vector3 From;
        public Vector3 To;
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

            var t = From + (To - From) * Sin() * _amplitude;

            transform.localScale = new Vector3(_scale.x * t.x, _scale.y * t.y, _scale.z * t.z);
     
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