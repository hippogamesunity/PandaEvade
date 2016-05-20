using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public class Swipe : Script
    {
        public EventDelegate OnLeft;
        public EventDelegate OnRight;
        public float Sensitivity;

        private bool _pressed;
        private Vector2 _position;

        public void Update()
        {
            if (Input.GetMouseButtonDown(0) && GetComponent<Collider2D>().bounds.Contains((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                _position = Input.mousePosition;
                _pressed = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _pressed = false;
            }

            if (_pressed)
            {
                var delta = _position - (Vector2) Input.mousePosition;

                if (Math.Abs(delta.x) > Sensitivity * Screen.width)
                {
                    if (delta.x > 0)
                    {
                        EventDelegate.Execute(new List<EventDelegate> { OnLeft });
                    }
                    else
                    {
                        EventDelegate.Execute(new List<EventDelegate> { OnRight });
                    }

                    _pressed = false;
                }
            }
        }
    }
}