using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Engine : Script
    {
        public TweenPanel MenuPanel;
        public TweenPanel ResultPanel;
        public TweenPanel[] IngamePanels;
        public GameObject Stage;
        public Branch[] Branches;
        public ParticleSystem[] Winds;
        public Transform Balls;
        public UILabel Score;
        public UILabel Result;
        public UIBasicSprite[] Hearts;
        public Panda Panda;

        public void Play()
        {
            TaskScheduler.CreateTask(() => CreateWind(Random.Range(0, 2)), Id, 2);
            Stage.transform.localPosition = new Vector3(0, -12, 0);
            TweenPosition.Begin(Stage, 0.5f, new Vector3(0, 0, 0));
            MenuPanel.Hide();
            TaskScheduler.CreateTask(() =>
            {
                foreach (var panel in IngamePanels)
                {
                    panel.Show();
                }
            }, Id, 0.5f);
            Panda.Reset();
        }

        public void Stop(int score)
        {
            TaskScheduler.Kill(Id);
            Result.SetText(score);
            Profile.Instance.BestScore = Math.Max(score, Profile.Instance.BestScore.Long);
            TweenPosition.Begin(Stage, 0.5f, new Vector3(0, -12, 0));
            ResultPanel.Show();

            foreach (var panel in IngamePanels)
            {
                panel.Hide();
            }
        }

        public void ReturnToMenu()
        {
            ResultPanel.Hide();
            MenuPanel.Show();
        }

        public void CreateWind(int side)
        {
            if (side == 1)
            {
                Branches[0].AnimateLeft();
                Branches[1].AnimateLeft();
                Winds[1].Play();
            }
            else
            {
                {
                    Branches[0].AnimateRight();
                    Branches[1].AnimateRight();
                    Winds[0].Play();
                }
            }

            TaskScheduler.CreateTask(() => CreateBall(side), Id, 0.5f);
            TaskScheduler.CreateTask(() => CreateWind(Random.Range(0, 2)), Id, Random.Range(1f, 2f));
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
        public void OpenLeaderboard()
        {
            Debug.Log("OpenLeaderboard");
            GamesServices.OpenLeaderboards(new Dictionary<string, long> { { GPGConstants.leaderboard_highscore, Profile.Instance.BestScore.Long } } );
        }

        public void OpenAchievements()
        {
            Debug.Log("OpenAchievements");
            GamesServices.OpenAchievements(new List<string>());
        }
    }
}