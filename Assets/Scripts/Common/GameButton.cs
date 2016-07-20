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
        public string SpriteEnabled;
        public string SpriteDisabled;
        public AnimationCurve AnimationCurve = new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 1, 0));

        [Header("Tap Delay")]
        public float TapTimeout = 0.25f;
        public bool Shared = true;

        public event Action OnDown = () => { };
        public event Action OnUp = () => { };
        public event Action OnCancel = () => { };

        public bool Pressed { get; private set; }

        private Texture _texture;
        protected float TapTime;
        protected static float TapTimeShared;

        private Collider2D _collider;

        public Collider2D Collider
        {
            get { return _collider ?? (_collider = GetComponent<Collider2D>()); }
        }

        private UISprite _sprite;

        public UISprite UISprite
        {
            get { return _sprite ?? (_sprite = GetComponent<UISprite>()); }
        }

        public void Awake()
        {
            if (string.IsNullOrEmpty(SpriteEnabled) && UISprite != null)
            {
                SpriteEnabled = UISprite.spriteName;
            }
        }

        public void OnDragOut()
        {
            if (Pressed)
            {
                Tween(false);
                Pressed = false;
                OnCancel();
            }
        }

        public bool Enabled
        {
            get { return Collider.enabled; }
            set { Enable(value, value ? TweenTimeUp : TweenTimeDown); }
        }

        public virtual void Enable(bool value, float duration)
        {
            if (Enabled == value) return;

            Collider.enabled = value;

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
                if (string.IsNullOrEmpty(SpriteEnabled))
                {
                    SpriteEnabled = UISprite.spriteName;
                }

                UISprite.spriteName = value ? SpriteEnabled : SpriteDisabled;
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