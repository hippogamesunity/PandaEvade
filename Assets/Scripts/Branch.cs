using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public class Branch : Script
    {
        public Animator Animator;
        public AnimationClip StandAnimation;
        public AnimationClip LeftAnimation;
        public AnimationClip RightAnimation;

        public void Start()
        {
            Stand();

            if (Camera.main.aspect >= 0.6)
            {
                var swift = GetComponent<Renderer>().bounds.size.x / 2f;
                var align = Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x < 0 ? 0 : 1, 1));

                align.z = 0;
                transform.position = align - new Vector3(0, swift) + new Vector3(transform.position.x < 0  ? -0.2f : 0.2f, 0.2f);
            }
        }

        public void Stand()
        {
            Animator.Play(StandAnimation.name);
        }

        public void AnimateLeft()
        {
            Animator.Play(LeftAnimation.name);
            TaskScheduler.CreateTask(Stand, Id, LeftAnimation.length);
        }

        public void AnimateRight()
        {
            Animator.Play(RightAnimation.name);
            TaskScheduler.CreateTask(Stand, Id, RightAnimation.length);
        }
    }
}