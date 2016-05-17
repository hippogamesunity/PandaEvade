using System;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public class Body : Script
    {
        public TweenRotation TweenRotation;

        private int _evasion;

        public void Update()
        {
            if (_evasion != 0) return;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _evasion = -1;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _evasion = +1;
            }

            if (_evasion != 0)
            {
                TweenRotation.to = new Vector3(0, 0, -20 * _evasion);
                TweenRotation.ResetToBeginning();
                TweenRotation.PlayForward();
            }
        }

        public void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (Math.Sign(_evasion) == Math.Sign(collision2D.rigidbody.velocity.x))
            {
                Destroy(collision2D.collider);
            }
            else
            {
                collision2D.rigidbody.velocity = new Vector2(-collision2D.rigidbody.velocity.x, collision2D.rigidbody.velocity.y) / 5;
            }
        }

        public void EvasionFinished()
        {
            _evasion = 0;
        }
    }
}