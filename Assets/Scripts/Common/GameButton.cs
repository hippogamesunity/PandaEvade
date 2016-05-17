using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public enum EventType
    {
        Down,
        Up
    }

    public class GameButton : Script
    {
        [Header("Event")]
        public EventType Type = EventType.Up;
        public MonoBehaviour Listener;
        public string Method;
        public string Params;

        [Header("Tween")]
        public float TweenTimeDown = 0.05f;
        public float TweenTimeUp = 0.1f;
        public float ScaleDown = 1.1f;
        public Color ColorDown = Color.white;
        public Color ColorUp = Color.white;
        public Color ColorDisabled = Color.white;
        public Texture TextureDisabled;
        public string SpriteDisabled;
        public AnimationCurve AnimationCurve = new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 1, 0));

        [Header("Tap Delay")]
        public float TapTimeout = 0.5f;
        public bool Shared = true;

        public event Action OnDown = () => {};
        public event Action OnUp = () => {};

        public bool Pressed { get; private set; }

        private string _sprite;
        private Texture _texture;
        protected float TapTime;
        protected static float TapTimeShared;

        public void OnDragOut()
        {
            if (Pressed && !GetComponent<Collider>().bounds.Contains(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                Tween(false);
                Pressed = false;
            }
        }

        public bool Enabled
        {
            get { return GetComponent<Collider>().enabled; }
            set { Enable(value, value ? TweenTimeUp : TweenTimeDown); }
        }

        public void Enable(bool value, float duration)
        {
            if (Enabled == value) return;

            GetComponent<Collider>().enabled = value;

            if (duration > 0)
            {
                TweenColor.Begin(gameObject, duration, value ? ColorUp : ColorDisabled).animationCurve = AnimationCurve;
                TweenScale.Begin(gameObject, duration, Vector3.one).animationCurve = AnimationCurve;
            }
            else
            {
                GetComponent<UIWidget>().color = value ? ColorUp : ColorDisabled;
                transform.localScale = Vector3.one;
            }

            if (TextureDisabled != null)
            {
                if (_texture == null)
                {
                    _texture = GetComponent<UITexture>().mainTexture;
                }

                GetComponent<UITexture>().mainTexture = value ? _texture : TextureDisabled;
            }

            if (!string.IsNullOrEmpty(SpriteDisabled))
            {
                if (_sprite == null)
                {
                    _sprite = GetComponent<UISprite>().spriteName;
                }

                GetComponent<UISprite>().spriteName = value ? _sprite : SpriteDisabled;
            }
        }

        public virtual void OnPress(bool down)
        {
            if (!enabled) return;

            Tween(down);

            if (down)
            {
                OnDown();

                if (Type == EventType.Down)
                {
                    SecureAction();
                }
            }
            else if (Pressed)
            {
                OnUp();

                if (Type == EventType.Up)
                {
                    SecureAction();
                }
            }

            Pressed = down;
        }

        protected bool SecureAction()
        {
            if (TapTimeout > 0)
            {
                if (Shared)
                {
                    if (Time.time - TapTimeShared > TapTimeout)
                    {
                        TapTimeShared = Time.time;
                        Action();
                        return true;
                    }
                }
                else if (Time.time - TapTime > TapTimeout)
                {
                    TapTime = Time.time;
                    Action();
                    return true;
                }
            }
            else
            {
                Action();
                return true;
            }

            return false;
        }

        protected virtual void Action()
        {
            if (Listener == null || Method == null) return;

            if (Params == null)
            {
                Listener.SendMessage(Method);
            }
            else
            {
                Listener.SendMessage(Method, Params);
            }
        }

        protected virtual void Tween(bool down)
        {
            if (down)
            {
                TweenColor.Begin(gameObject, TweenTimeDown, ColorDown).animationCurve = AnimationCurve;
                TweenScale.Begin(gameObject, TweenTimeDown, ScaleDown * Vector3.one).animationCurve = AnimationCurve;

            }
            else
            {
                TweenColor.Begin(gameObject, TweenTimeUp, ColorUp).animationCurve = AnimationCurve;
                TweenScale.Begin(gameObject, TweenTimeUp, Vector3.one).animationCurve = AnimationCurve;
            }
        }
    }
}