using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class TaskScheduler : Script
    {
        private class Task
        {
            public long Id;
            public Action Action;
            public Action Callback;
        }

        private static readonly List<Task> Tasks = new List<Task>();

        private static TaskScheduler _instance;

        private static TaskScheduler Instance
        {
            get { return _instance ?? (_instance = new GameObject("TaskScheduler").AddComponent<TaskScheduler>()); }
        }

        public static void CreateTask(Action action, long id, float delay)
        {
            CreateTask(action, id, delay, null);
        }

        public static long CreateTask(Action action, float delay)
        {
            var id = CRandom.GetRandom(999999, 9999999);

            CreateTask(action, id, delay, null);

            return id;
        }

        public static void CreateTask(Action action, float delay, Action callback)
        {
            CreateTask(action, CRandom.GetRandom(999999), delay, callback);
        }

        public static void CreateTask(Action action, long id, float delay, Action callback)
        {
            if (delay <= 0)
            {
                action();

                if (callback != null)
                {
                    callback();
                }
            }
            else
            {
                var task = new Task { Id = id, Action = action, Callback = callback };

                Tasks.Add(task);
                Instance.StartCoroutine(Coroutine(task, delay));
            }
        }

        public static void Kill(params int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                Tasks.Clear();
            }
            else
            {
                foreach (var id in ids)
                {
                    Tasks.RemoveAll(i => i.Id == id);
                }
            }
        }

        public void OnDestroy()
        {
            Kill();
            _instance = null;
        }

        private static IEnumerator Coroutine(Task task, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (!Tasks.Contains(task)) yield break;

            task.Action();

            if (task.Callback != null)
            {
                task.Callback();
            }

            Tasks.Remove(task);
        }
    }
}