using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public class Beads : Script
    {
        public Animator BeadAnimator;
        public AnimationClip BeadStandAnimation;
        public AnimationClip BeadFallAnimation;

        public void Fall()
        {
            BeadAnimator.Play(BeadFallAnimation.name);
            TaskScheduler.Kill(Id);
            TaskScheduler.CreateTask(() => BeadAnimator.Play(BeadStandAnimation.name), Id, BeadFallAnimation.length);
        }
    }
}
