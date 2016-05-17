using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public class Engine : Script
    {
        public TweenColor[] Indicators;
        public Transform Balls;

        public void Start()
        {
            TaskScheduler.CreateTask(() => CreateWind(Random.Range(0, 2)), 1);
        }

        public void CreateWind(int side)
        {
            Indicators[side].ResetToBeginning();
            Indicators[side].PlayForward();
            TaskScheduler.CreateTask(() => CreateBall(side), 0.5f);
            TaskScheduler.CreateTask(() => CreateWind(Random.Range(0, 2)), 1);
        }

        public void CreateBall(int side)
        {
            var ball = PrefabsHelper.Instantiate("Ball", Balls);

            ball.transform.localPosition = new Vector2(1000 * (side == 0 ? -1 : 1), 0);
            ball.GetComponent<Rigidbody2D>().AddForce(new Vector2(300 * (side == 0 ? 1 : -1), 100));
            Destroy(ball, 5);
        }
    }
}