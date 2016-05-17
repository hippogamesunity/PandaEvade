using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class SelectButton : GameButton
    {
        public event Action OnSelect = () => { };
        public event Action OnRelease = () => { };
        public event Action OnConfirm = () => { };

        public int Tag = 0;
        public bool Selected;

        private static readonly SortedDictionary<int, SelectButton> SelectedButtons = new SortedDictionary<int, SelectButton>();
        private bool _skip;

        public void Start()
        {
            if (Selected)
            {
                OnPress();
            }
        }

        public new void OnDragOut()
        {
            if (Type == EventType.Up && !GetComponent<Collider>().bounds.Contains(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                _skip = true;
            }
        }

        public void Select()
        {
            if (!Selected)
            {
                OnPress();
            }
        }

        public void Release()
        {
            if (!Selected) return;

            SelectedButtons.Remove(Tag);
            Tween(false);
            Selected = false;
        }

        public override void OnPress(bool down)
        {
            if (Type == EventType.Up && !down)
            {
                if (_skip)
                {
                    _skip = false;
                }
                else
                {
                    OnPress();
                }
            }
            else if (Type == EventType.Down && down)
            {
                OnPress();
            }

            //if (enabled && down)
            //{
            //    OnPress();
            //}
        }

        public void OnPress()
        {
            if (!SecureAction()) return;

            OnSelect();

            if (SelectedButtons.ContainsKey(Tag))
            {
                if (SelectedButtons[Tag] == this)
                {
                    OnConfirm();
                }
                else if (SelectedButtons[Tag] != null)
                {
                    SelectedButtons[Tag].Tween(false);
                    SelectedButtons[Tag].OnRelease();
                    SelectedButtons[Tag].Selected = false;
                }
            }

            Tween(true);
            SelectedButtons[Tag] = this;
            Selected = true;

            if (Shared)
            {
                TapTimeShared = Time.time;
            }
            else
            {
                TapTime = Time.time;
            }
        }
    }
}