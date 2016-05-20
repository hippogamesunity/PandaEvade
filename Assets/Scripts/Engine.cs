using System;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Tweens;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Engine : Script
    {
        public ScaleSpring[] Indicators;
        public Transform Balls;
        public UILabel Score;

        private int _score;

        public void Start()
        {
            TaskScheduler.CreateTask(() => CreateWind(Random.Range(0, 2)), Random.Range(1f, 2f));
        }

        public void CreateWind(int side)
        {
            Indicators[side].Reset();
            Indicators[side].enabled = true;
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

        public void AddScore(int value)
        {
            _score = Math.Max(0, _score + value);
            Score.text = _score.ToString();
        }

        public int GetScore()
        {
            return _score;
        }
    }
}