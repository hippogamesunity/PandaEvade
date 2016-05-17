using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public enum TweenPanelState
    {
        Hidden,
        Hiding,
        Opened,
        Opening
    }

    public class TweenPanel : Script
    {
        public TweenDirection TweenDirection;
        public bool UseCustomTweenPosition;
        public Vector3 CustomTweenPosition;
        public float DefaultTimeout = 0.4f;
        public AnimationCurve AnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        public bool Transparency;

        public TweenPanelState State;

        public void Start()
        {
            State = TweenPanelState.Opened;
        }
     
        public void Show()
        {
            Tween(true, DefaultTimeout, TweenDirection);
        }

        public void Show(float timeout)
        {
            Tween(true, timeout, TweenDirection);
        }

        public void Show(TweenDirection direction)
        {
            Tween(true, DefaultTimeout, direction);
        }

        public void Hide()
        {
            Tween(false, DefaultTimeout, TweenDirection);
        }

        public void Hide(float timeout)
        {
            Tween(false, timeout, TweenDirection);
        }

        public void Hide(TweenDirection direction)
        {
            Tween(false, DefaultTimeout, direction);
        }
    
        public void Hide(TweenDirection direction, float timeout)
        {
            Tween(false, timeout, direction);
        }

        private void Tween(bool open, float timeout, TweenDirection tweenDirection)
        {
            if (open && (State == TweenPanelState.Opened || State == TweenPanelState.Opening)) return;
            if (!open && (State == TweenPanelState.Hidden || State == TweenPanelState.Hiding)) return;

            TaskScheduler.Kill(Id);

            State = open ? TweenPanelState.Opening : TweenPanelState.Hiding;
            TaskScheduler.CreateTask(() => State = open ? TweenPanelState.Opened : TweenPanelState.Hidden, Id, timeout);

            Vector3 target;

            if (open && State != TweenPanelState.Hiding)
            {
                transform.localPosition = GetHiddenPosition(tweenDirection);

                if (Transparency)
                {
                    GetComponent<UIPanel>().alpha = 0;
                }
            }

            if (open)
            {
                target = Vector3.zero;
                gameObject.SetActive(true);
            }
            else
            {
                target = GetHiddenPosition(tweenDirection);

                if (timeout > 0)
                {
                    TaskScheduler.CreateTask(() => gameObject.SetActive(false), Id, timeout);
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }

            if (timeout > 0)
            {
                TweenPosition.Begin(gameObject, timeout, target).animationCurve = AnimationCurve;

                if (Transparency)
                {
                    TaskScheduler.CreateTask(() => TweenAlpha.Begin(gameObject, timeout / 2, open ? 1 : 0), Id, open ? timeout / 2 : 0);
                }
            }
            else
            {
                transform.localPosition = target;

                if (Transparency)
                {
                    GetComponent<UIPanel>().alpha = open ? 1 : 0;
                }
            }
        }

        private Vector3 GetHiddenPosition(TweenDirection tweenDirection)
        {
            if (UseCustomTweenPosition)
            {
                return new Vector3(CustomTweenPosition.x * transform.localScale.x, CustomTweenPosition.y * transform.localScale.y);
            }

            var aspect = 1000 * Camera.main.aspect;

            switch (tweenDirection)
            {
                case TweenDirection.Left:
                    return -Vector2.right * aspect;
                case TweenDirection.Right:
                    return Vector2.right * aspect;
                case TweenDirection.Up:
                    return Vector2.up * 1000;
                case TweenDirection.Down:
                    return -Vector2.up * 1000;
                default:
                    throw new Exception();
            }
        }
    }

    public enum TweenDirection { Left, Right, Up, Down }
}