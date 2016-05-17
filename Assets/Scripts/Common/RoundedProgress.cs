using UnityEngine;

namespace Assets.Scripts.Common
{
    public class RoundedProgress : Script
    {
        public UIWidget Begin;
        public UIWidget End;
        public UIWidget Progress;

        public void Awake()
        {
            Reset();
        }

        public void Go(float duration)
        {
            Begin.enabled = End.enabled = Progress.enabled = true;
            End.transform.position = Begin.transform.position;
            Progress.transform.localScale = new Vector3(0, 1);

            TweenScale.Begin(Progress.gameObject, duration, Vector3.one);
            TweenPosition.Begin(End.gameObject, duration, new Vector3(-Begin.transform.localPosition.x, 0));
        }

        public void Set(float value)
        {
            value = Mathf.Min(1, value);
            Begin.enabled = End.enabled = Progress.enabled = true;
            Progress.transform.localScale = new Vector3(value, 1);
            End.transform.localPosition = new Vector3(Begin.transform.localPosition.x - 2 * value * Begin.transform.localPosition.x, 0);
        }

        public void Reset()
        {
            Progress.transform.localScale = new Vector3(0, 1);
            End.transform.position = Begin.transform.position;
            Begin.enabled = End.enabled = Progress.enabled = false;
        }
    }
}