using System;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public class Panda : Script
    {
        public Transform Stage;
        public Collider2D[] Colliders;
        public UI UI;
        public Engine Engine;
        public Beads Beads;
        public Animator Animator;
        public AnimationClip StandAnimation;
        public AnimationClip EvadeAnimation;
        public AnimationClip HitAnimation;
        public AnimationClip FallAnimation;
        
        private int _evasion;
        private int _hit;
        
        public int Score { get; private set; }
        public int Hearts { get; private set; }
        
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

        public void Reset()
        {
            UI.UpdateScore(Score = 0);
            UI.UpdateHearts(Hearts = 3);
            Stand();

            foreach (var c in Colliders)
            {
                c.enabled = true;
            }
        }

        public void Continue()
        {
            UI.UpdateHearts(Hearts = 0);
            Stand();
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

        public void OnTriggerEnter2D(Collider2D c)
        {
            if (_hit != 0) return;

            var sign = Math.Sign(c.GetComponent<Rigidbody2D>().velocity.x);

            if (Math.Sign(_evasion) == sign)
            {
                UI.UpdateScore(++Score);
            }
            else
            {
                UI.UpdateHearts(--Hearts);
                c.gameObject.GetComponent<Ball>().Recoil(-sign);
                _hit = -sign;
                SetReflection(_hit);

                if (Hearts >= 0)
                {
                    TaskScheduler.Kill(Id);
                    Animator.Play(HitAnimation.name);
                    TaskScheduler.CreateTask(Stand, Id, HitAnimation.length);
                }
                else
                {
                    TaskScheduler.Kill(Id);
                    Animator.Play(FallAnimation.name);
                    Beads.Fall();
                    TaskScheduler.CreateTask(() => Engine.Stop(Score), Id, 2);
                }
            }
        }

        public void Fall()
        {
            foreach (var c in Colliders)
            {
                c.enabled = false;
            }

            TaskScheduler.Kill(Id);
            Animator.Play(FallAnimation.name);
            Beads.Fall();
        }

        private void Stand()
        {
            _evasion = _hit = 0;
            SetReflection(1);
            Animator.Play(StandAnimation.name);
        }

        private void Evade()
        {
            if (!Colliders[0].enabled) return;

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