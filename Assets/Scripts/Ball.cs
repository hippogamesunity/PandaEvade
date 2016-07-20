using UnityEngine;

namespace Assets.Scripts
{
    public class Ball : MonoBehaviour
    {
        public float Torque;

        public bool Hit { get; private set; }

        public void Initialize(int side)
        {
            transform.localPosition = new Vector2(20 * (side == 0 ? -1 : 1), 0);
            transform.localScale = new Vector2(1, 1);
            GetComponent<Rigidbody2D>().AddForce(new Vector2(2000 * (side == 0 ? 1 : -1), Random.Range(0, 300)));
            GetComponent<Rigidbody2D>().AddTorque(Torque);
            gameObject.layer = LayerMask.NameToLayer(side == 1 ? "CollisionR" : "CollisionL");
        }

        public void OnTriggerEnter2D(Collider2D c)
        {
            Destroy(GetComponent<Collider2D>());
        }

        public void Recoil(int direction)
        {
            Hit = true;
            GetComponent<Rigidbody2D>().velocity = new Vector2(10 * direction, -10);
        }
    }
}