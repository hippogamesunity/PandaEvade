using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Assets.Scripts
{
    public class UI : Script
    {
        public Engine Engine;
        public TweenPanel[] MenuPanels;
        public TweenPanel ContinuePanel;
        public TweenPanel ResultPanel;
        public TweenPanel[] IngamePanels;
        public TweenPanel ItemUnlockedPanel;
        public UISprite UnlockedItem;
        public UIBasicSprite[] Hearts;
        public GameObject Warning;
        public UILabel Score;
        public UILabel MaxScore;
        public UILabel Result;
        public UISprite SoundButton;
        public UISprite PauseButton;
        public GameObject AbortButton;
        public GameObject SwipeTip;

        private float _repostTime;

        public void Start()
        {
            UpdateSettings();
            OpenMenu();
            StartCoroutine(SwipeLoop(1));
            Advertisement.Initialize(Application.platform == RuntimePlatform.Android ? "1076481" : "1098700");
        }

        private bool _forward;

        private IEnumerator SwipeLoop(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (MenuPanels[1].State == TweenPanelState.Opened)
            {
                _forward = !_forward;

                foreach (var tween in SwipeTip.GetComponents<UITweener>())
                {
                    if (_forward)
                    {
                        tween.PlayForward();
                    }
                    else
                    {
                        tween.PlayReverse();
                    }
                }
            }

            StartCoroutine(SwipeLoop(2));
        }

        public void OpenIngame()
        {
            foreach (var p in MenuPanels)
            {
                p.Hide();
            }

            AbortButton.GetComponent<GameButton>().Enabled = true;
            PauseButton.GetComponent<GameButton>().Enabled = true;

            TaskScheduler.CreateTask(() => { foreach (var panel in IngamePanels) panel.Show(); }, Id, 0.5f);
        }

        public void OpenResult(int score, bool continued)
        {
            Result.SetText(score);
            MaxScore.SetText("MAX: {0}", Profile.Instance.BestScore);

            foreach (var panel in IngamePanels)
            {
                panel.Hide();
            }

            if (Advertisement.IsReady() && !continued)
            {
                ContinuePanel.Show();
            }
            else
            {
                OpenResult();
            }
        }

        public void CloseItemUnlocked()
        {
            ItemUnlockedPanel.Hide();
            ResultPanel.Show();
        }

        public void SkipAds()
        {
            ContinuePanel.Hide();
            OpenResult();
        }

        public void OpenResult()
        {
            if (Profile.Instance.UnlockedItems.Contains(Engine.Panda.Item))
            {
                ResultPanel.Show();
            }
            else
            {
                Profile.Instance.UnlockedItems.Add(Engine.Panda.Item);
                Profile.Instance.Save();
                UnlockedItem.spriteName = Engine.Panda.Item.ToString();
                ItemUnlockedPanel.Show();
                AudioPlayer.Instance.PlayEffect(AudioPlayer.Instance.ItemUnlocked);
                AppMetrica.Instance.ReportEvent("ItemUnlocked", new Dictionary<string, object> { { "Item", Engine.Panda.Item.ToString() } });
            }
        }

        public void ShowAds()
        {
            ContinuePanel.Hide();

            TaskScheduler.CreateTask(() =>
            {
                Advertisement.Show("rewardedVideo", new ShowOptions
                {
                    resultCallback = result =>
                    {
                        if (result == ShowResult.Finished)
                        {
                            TaskScheduler.CreateTask(Engine.Continue, Id, 1);
                            AppMetrica.Instance.ReportEvent("AdWatch");
                        }
                        else
                        {
                            TaskScheduler.CreateTask(ContinuePanel.Show, Id, 1);
                        }
                    }
                });
            }, Id, 1);
        }

        public void ReturnToMenu()
        {
            ResultPanel.Hide();
            OpenMenu();
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

            Warning.SetActive(hearts <= 0);
        }

        public void Pause()
        {
            Time.timeScale = Time.timeScale > 0 ? 0 : 1;
            PauseButton.spriteName = Time.timeScale > 0 ? "Pause" : "Resume";
        }

        public void Abort()
        {
            Time.timeScale = 1;
            AbortButton.GetComponent<GameButton>().Enabled = false;
            PauseButton.GetComponent<GameButton>().Enabled = false;

            Engine.Panda.Fall();

            TaskScheduler.CreateTask(() => { Engine.Abort(); OpenMenu(); }, Id, 2);

            foreach (var panel in IngamePanels)
            {
                panel.Hide();
            }
        }

        public void Rate()
        {
            Application.OpenURL(PlanformDependedSettings.StoreLink);
            AppMetrica.Instance.ReportEvent("Rate");
        }

        public void Mute()
        {
            Profile.Instance.Sound = !Profile.Instance.Sound.Bool;
            Profile.Instance.Save();
            AudioPlayer.Instance.SetVolume(Profile.Instance.Sound.Bool);
            UpdateSettings();
        }

        public void OpenLeaderboard()
        {
            #if UNITY_ANDROID

            Action action = () => GooglePlayGames.PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGConstants.leaderboard_player_scores);

            #else

            Action action = Social.ShowLeaderboardUI;

            #endif

            var scores = new Dictionary<string, long> { { GPGConstants.leaderboard_player_scores, Profile.Instance.BestScore.Long } };

            GamesServices.PostScores(scores, (success, exception) => { if (success) { action(); AppMetrica.Instance.ReportEvent("PostScores"); } });
        }

        public void OpenAchievements()
        {
            var achievements = new List<string>();

            if (Profile.Instance.UnlockedItems.Contains(BallId.Beach)) achievements.Add(GPGConstants.achievement_beach_ball);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Tennis)) achievements.Add(GPGConstants.achievement_tennis_ball);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Tomato)) achievements.Add(GPGConstants.achievement_tomato);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Boomerang)) achievements.Add(GPGConstants.achievement_boomerang);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Rugby)) achievements.Add(GPGConstants.achievement_rugby_ball);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Football)) achievements.Add(GPGConstants.achievement_football_ball);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Shoe)) achievements.Add(GPGConstants.achievement_sneaker);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Banana)) achievements.Add(GPGConstants.achievement_banana);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Lifebuoy)) achievements.Add(GPGConstants.achievement_lifebuoy);
            if (Profile.Instance.UnlockedItems.Contains(BallId.ToiletPaper)) achievements.Add(GPGConstants.achievement_toilet_paper);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Cactus)) achievements.Add(GPGConstants.achievement_cactus);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Flowerpot)) achievements.Add(GPGConstants.achievement_flowerpot);
            if (Profile.Instance.UnlockedItems.Contains(BallId.LightBeer)) achievements.Add(GPGConstants.achievement_light_beer_bottle);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Bread)) achievements.Add(GPGConstants.achievement_loaf);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Icecream)) achievements.Add(GPGConstants.achievement_icecream);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Hammer)) achievements.Add(GPGConstants.achievement_hammer);
            if (Profile.Instance.UnlockedItems.Contains(BallId.PremiumBeer)) achievements.Add(GPGConstants.achievement_premium_beer_bottle);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Pokeball)) achievements.Add(GPGConstants.achievement_pookeball);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Pikachu)) achievements.Add(GPGConstants.achievement_pookachu);
            if (Profile.Instance.UnlockedItems.Contains(BallId.Grenade)) achievements.Add(GPGConstants.achievement_grenade);

            GamesServices.PostAchievements(achievements, (success, exception) => { if (success) { Social.ShowAchievementsUI(); AppMetrica.Instance.ReportEvent("PostAchievements"); } });
        }

        private void UpdateSettings()
        {
            SoundButton.spriteName = Profile.Instance.Sound.Bool ? "Sound" : "NoSound";
        }

        private void OpenMenu()
        {
            foreach (var p in MenuPanels)
            {
                p.Show();
            }

            Engine.Panda.Reset();
            TweenPosition.Begin(Engine.Stage, 0.5f, Vector3.zero);
        }
    }
}