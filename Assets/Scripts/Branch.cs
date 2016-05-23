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