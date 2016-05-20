using System;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public class Panda : Script
    {
        public Transform Stage;
        public Engine Engine;
        public Beads Beads;
        public Animator Animator;
        public AnimationClip StandAnimation;
        public AnimationClip EvadeAnimation;
        public AnimationClip HitAnimation;
        public AnimationClip FallAnimation;
        
        private int _evasion;
        private int _hit;

        public void Start()
        {
            Animator.Play(StandAnimation.name);
        }

        public void Update()
        {
            if (_evasion != 0 || _hit != 0) return;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                EvadeLeft();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                EvadeRight();
            }
        }

        public void EvadeLeft()
        {
            if (_evasion == 0 && _hit == 0)
            {
                _evasion = -1;
                Evade();
            }
        }

        public void EvadeRight()
        {
            if (_evasion == 0 && _hit == 0)
            {
                _evasion = +1;
                Evade();
            }
        }

        public void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (_hit != 0) return;

            var sign = Math.Sign(collision2D.rigidbody.velocity.x);

            if (Math.Sign(_evasion) == sign)
            {
                Engine.AddScore(1);
            }
            else
            {
                Engine.AddScore(-5);
                collision2D.rigidbody.velocity = new Vector2(10 * -sign, -10);
                _hit = -sign;
                SetReflection(_hit);

                if (Engine.GetScore() > 0)
                {
                    Animator.Play(HitAnimation.name);
                    TaskScheduler.Kill(Id);
                    TaskScheduler.CreateTask(Stand, Id, HitAnimation.length);
                }
                else
                {
                    Animator.Play(FallAnimation.name);
                    TaskScheduler.Kill(Id);
                    TaskScheduler.CreateTask(Stand, Id, 2);
                    Beads.Fall();
                }
            }
        }

        private void Stand()
        {
            _evasion = _hit = 0;
            SetReflection(1);
            Animator.Play(StandAnimation.name);
        }

        private void Evade()
        {
            SetReflection(-_evasion);
            Animator.Play(EvadeAnimation.name);
            TaskScheduler.CreateTask(Stand, Id, EvadeAnimation.length);
        }

        private void SetReflection(int invert)
        {
            var scale = Stage.localScale;

            Stage.localScale = new Vector3(invert * Math.Abs(scale.x), scale.y, scale.z);
        }
    }
}