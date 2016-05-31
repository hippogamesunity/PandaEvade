using System;
using System.Collections.Generic;
using System.Linq;
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

        private int _count;
        private readonly List<float> _ballTimes = new List<float>(); 

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
            _count = 0;
            _ballTimes.Clear();
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
            const float ballDelay = 0.5f;

            if (Panda.Hearts < 0) return;

            if (_ballTimes.All(i => i < Time.time || (Time.time - i) > ballDelay))
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

                TaskScheduler.CreateTask(() => CreateBall(side), Id, ballDelay);
            }

            var delay = Mathf.Max(0.5f, 2 - _count / 100f);

            TaskScheduler.CreateTask(() => CreateWind(Random.Range(0, 2)), Id, Random.Range(delay, 1.5f * delay));
        }

        public void CreateBall(int side)
        {
            const float returnTime = 1;

            var distribution = new Dictionary<BallId, int>
            {
                { BallId.TennisBall, 10 }
            };
            var collection = new List<BallId>();

            if (_count > 5)
            {
                distribution.Add(BallId.Boomerang, Math.Max(10, _count - 5));
            }

            if (_count > 10)
            {
                distribution.Add(BallId.FootballBall, Math.Max(10, _count - 10));
            }

            foreach (var item in distribution)
            {
                for (var i = 0; i < item.Value; i++)
                {
                    collection.Add(item.Key);
                }
            }

            var ballId = collection[CRandom.GetRandom(collection.Count)];
            var ball = CreateBall(ballId, side);

            if (ballId == BallId.Boomerang)
            {
                TaskScheduler.CreateTask(() => { if (Panda.Hearts >= 0 && !ball.Hit) CreateBall(BallId.Boomerang, side == 0 ? 1 : 0); }, Id, returnTime);
                _ballTimes.Add(Time.time + returnTime);
            }
            else if (ballId == BallId.FootballBall)
            {
                TaskScheduler.CreateTask(() => { if (Panda.Hearts >= 0) CreateBall(BallId.FootballShoe, side); }, Id, returnTime);
                _ballTimes.Add(Time.time + returnTime);
            }

            _count++;
            _ballTimes.Add(Time.time);
        }

        private Ball CreateBall(BallId ballId, int side)
        {
            var instance = PrefabsHelper.Instantiate("Balls/" + ballId, Balls);
            var ball = instance.GetComponent<Ball>();

            ball.Initialize(side);
            Destroy(instance, 5);

            return ball;
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