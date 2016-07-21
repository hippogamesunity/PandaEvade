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
        public UI UI;
        public GameObject Stage;
        public Branch[] Branches;
        public ParticleSystem[] Winds;
        public Transform Balls;
        public Panda Panda;

        private int _progress;
        private bool _continue;
        private readonly List<float> _ballTimes = new List<float>();

        public void Start()
        {
            AudioPlayer.Instance.SetVolume(Profile.Instance.Sound.Bool);
            AudioPlayer.Instance.PlayMusic(GameMusic.Ingame);
        }

        public void Update()
        {
            #if UNITY_EDITOR

            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("RESET");
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }

            #endif

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (UI.MenuPanels[0].State == TweenPanelState.Opened)
                {
                    Application.Quit();
                }
                else if (UI.IngamePanels[0].State == TweenPanelState.Opened)
                {
                    UI.Abort();
                }
            }
        }

        public void OnApplicationPause(bool pause)
        {
            if (pause) Profile.Instance.Save();
        }

        public void OnApplicationQuit()
        {
            Profile.Instance.Save();
        }

        public void Play()
        {
            TaskScheduler.CreateTask(() => CreateWind(0), Id, 3);
            UI.OpenIngame();
            Panda.Reset();
            _progress = 0;
            _continue = false;
            _ballTimes.Clear();
        }

        public void Continue()
        {
            TaskScheduler.CreateTask(() => CreateWind(Random.Range(0, 2)), Id, 2);
            Stage.transform.localPosition = new Vector3(0, -15, 0);
            TweenPosition.Begin(Stage, 0.5f, new Vector3(0, 0, 0));
            UI.OpenIngame();
            Panda.Continue();
            _continue = true;
            _ballTimes.Clear();
        }

        public void Stop(int score)
        {
            TaskScheduler.Kill(Id);
            Profile.Instance.BestScore = Math.Max(score, Profile.Instance.BestScore.Long);
            TweenPosition.Begin(Stage, 0.5f, new Vector3(0, -15, 0));
            UI.OpenResult(score, _continue);
        }

        public void Abort()
        {
            TaskScheduler.Kill(Id);
            TweenPosition.Begin(Stage, 0.5f, new Vector3(0, -15, 0));

            foreach (var ball in FindObjectsOfType<Ball>())
            {
                Destroy(ball.gameObject);
            }
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

            var delay = Mathf.Max(0.25f, 2 - _progress / 100f);

            TaskScheduler.CreateTask(() => CreateWind(Random.Range(0, 2)), Id, Random.Range(delay, 1.5f * delay));
        }

        public void CreateBall(int side)
        {
            const float returnTime = 1;

            var distribution = new Dictionary<BallId, int>
            {
                { BallId.Beach, 1 },
                { BallId.Tennis, 1 },
                { BallId.Tomato, 1 }
            };
            var collection = new List<BallId>();

            if (_progress > 5) distribution.Add(BallId.Boomerang, 1);
            if (_progress > 10) distribution.Add(BallId.Rugby, 1);
            if (_progress > 15) distribution.Add(BallId.Football, 1);
            if (_progress > 20) distribution.Add(BallId.Banana, 1);
            if (_progress > 25) distribution.Add(BallId.ToiletPaper, 1);
            if (_progress > 30) distribution.Add(BallId.Cactus, 1);
            if (_progress > 35) distribution.Add(BallId.LightBeer, 1);
            if (_progress > 40) distribution.Add(BallId.Icecream, 1);
            if (_progress > 45) distribution.Add(BallId.Hammer, 1);
            if (_progress > 50) distribution.Add(BallId.Pokeball, 1);
            if (_progress > 55) distribution.Add(BallId.PremiumBeer, 1);
            if (_progress > 60) distribution.Add(BallId.Grenade, 1);
            
            foreach (var item in distribution)
            {
                for (var i = 0; i < item.Value; i++)
                {
                    collection.Add(item.Key);
                }
            }

            var ballId = collection[CRandom.GetRandom(collection.Count)];
            var ball = CreateBall(ballId, side);

            switch (ballId)
            {
                case BallId.Boomerang:
                    TaskScheduler.CreateTask(() => { if (Panda.Hearts >= 0 && !ball.Hit) CreateBall(BallId.Boomerang, side == 0 ? 1 : 0); }, Id, returnTime);
                    _ballTimes.Add(Time.time + returnTime);
                    break;
                case BallId.Pokeball:
                    TaskScheduler.CreateTask(() => { if (Panda.Hearts >= 0 && !ball.Hit) CreateBall(BallId.Pikachu, side == 0 ? 1 : 0); }, Id, returnTime);
                    _ballTimes.Add(Time.time + returnTime);
                    break;
                case BallId.Football:
                    TaskScheduler.CreateTask(() => { if (Panda.Hearts >= 0) CreateBall(BallId.Shoe, side); }, Id, returnTime);
                    _ballTimes.Add(Time.time + returnTime);
                    break;
                case BallId.Cactus:
                    TaskScheduler.CreateTask(() => { if (Panda.Hearts >= 0) CreateBall(BallId.Flowerpot, side); }, Id, returnTime);
                    _ballTimes.Add(Time.time + returnTime);
                    break;
            }

            _progress++;
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
    }
}