using Assets.Scripts.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Engine : Script
    {
        public Branch[] Branches;
        public Transform Balls;
        public UILabel Score;
        public UIBasicSprite[] Hearts;

        public void Start()
        {
            TaskScheduler.CreateTask(() => CreateWind(Random.Range(0, 2)), Random.Range(1f, 2f));
        }

        public void CreateWind(int side)
        {
            if (side == 1)
            {
                Branches[0].AnimateLeft();
                Branches[1].AnimateLeft();
            }
            else
            {
                {
                    Branches[0].AnimateRight();
                    Branches[1].AnimateRight();
                }
            }

            TaskScheduler.CreateTask(() => CreateBall(side), 0.5f);
            TaskScheduler.CreateTask(() => CreateWind(Random.Range(0, 2)), Random.Range(1f, 2f));
        }

        public void CreateBall(int side)
        {
            var ball = PrefabsHelper.Instantiate("Ball", Balls);

            ball.transform.localPosition = new Vector2(20 * (side == 0 ? -1 : 1), 0);
            ball.transform.localScale = new Vector2(1, 1);
            ball.GetComponent<Rigidbody2D>().AddForce(new Vector2(2000 * (side == 0 ? 1 : -1), Random.Range(0, 300)));
            ball.GetComponent<Rigidbody2D>().AddTorque(200);
            Destroy(ball, 5);
        }

        public void UpdateScore(int score)
        {
            Score.SetText(score);
        }

        public void UpdateHearts(int hearts)
        {
            for (var i = 0; i < Hearts.Length; i++)
            {
                Hearts[i].enabled = hearts > i;
            }
        }
    }
}