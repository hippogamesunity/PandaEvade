using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SimpleAds.Service
{
    // We need ThreadSaveExecutor because some ad plugins have callbacks from another threads.
    // https://stackoverflow.com/questions/41330771/use-unity-api-from-another-thread-or-call-a-function-in-the-main-thread
    // https://answers.unity.com/questions/1689879/admoob-rewardbasedvideoad-app-crash-after-reward-v.html
    public class ThreadSaveExecutor : MonoBehaviour
    {
        public static List<Action> Tasks = new List<Action>();
        public static ThreadSaveExecutor Instance;

        public void Awake()
        {
            Instance = this;
        }

        public static void Execute(Action task)
        {
            if (Instance == null)
            {
                Instance = new GameObject(typeof(ThreadSaveExecutor).Name).AddComponent<ThreadSaveExecutor>();
            }

            if (task == null) return;

            Tasks.Add(task);
            Instance.enabled = true;
        }

        public void Update()
        {
            while (Tasks.Count > 0)
            {
                var task = Tasks[0];

                task?.Invoke();
                Tasks.RemoveAt(0);
            }

            enabled = false;
        }
    }
}