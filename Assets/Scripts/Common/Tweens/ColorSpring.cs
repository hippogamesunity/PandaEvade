using UnityEngine;

namespace Assets.Scripts.Common.Tweens
{
    public class ColorSpring : TweenBase
    {
        public Color From;
        public Color To;

        private UIWidget _widget;

        public void Awake()
        {
            _widget = GetComponent<UIWidget>();
        }

        public void Update()
        {
            _widget.color = From + (To - From) * Sin();
        }

        public void OnDisable()
        {
            _widget.color = From;
        }
    }
}