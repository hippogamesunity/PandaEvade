using Assets.Scripts.Common;
using Assets.Scripts.Common.Tweens;
using UnityEngine;

namespace Assets.Scripts
{
    public class Ball : MonoBehaviour
    {
        public Rigidbody2D Rigidbody;

        //public void Update()
        //{
        //    transform.rotation = Quaternion.Euler(0, 0, 90 + RotationHelper.Angle(Rigidbody.velocity));
        //}

        public void OnCollisionEnter2D(Collision2D collision2D)
        {
            Destroy(GetComponent<Collider2D>());
            //TweenScale.Begin(gameObject, 0.2f, new Vector3(1, 1, 1));
        }
    }
}