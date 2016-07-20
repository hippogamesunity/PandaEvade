using UnityEngine;

namespace Assets.Scripts
{
    public class PandaCollider : MonoBehaviour
    {
        public Panda Panda;

        public void OnTriggerEnter2D(Collider2D c)
        {
            var a = this;

            Panda.OnTriggerEnter2D(c);
        }
    }
}