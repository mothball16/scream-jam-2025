using Assets.Scripts.Events;
using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Tools
{
    internal class TimedTask
    {
        public float Time { get; set; }
        public Action Callback { get; set; }

        public TimedTask(float time, Action callback)
        {
            Time = time;
            Callback = callback;
        }
    }
    internal class Timer : MonoSingleton<Timer>
    {
        private List<IDisposable> _subscriptions;
        private Dictionary<string, TimedTask> _tasks;
        private void OnEnable()
        {
            _tasks = new();
            _subscriptions = new() {
                EventBus.Subscribe<SetTimedEvent>(OnTimerSet),
                EventBus.Subscribe<CancelTimedEvent>(OnTimerCancelled),
            };
        kjjkjjkjljkkjjkjkjjjkjkkkjjj}
        private void OnDisable()
        {jk
            _tasks.Clear();
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions.Clear();
        }

        private void OnTimerSet(SetTimedEvent e)
        {
            _tasks[e.ID] = new(e.Time, e.Callback);
        }

        private void OnTimerCancelled(CancelTimedEvent e)
        {
            if(_tasks.ContainsKey(e.ID))
                _tasks.Remove(e.ID);
        }

        private void Update()
        {
            var toRemove = new List<string>();
            foreach(var kvp in _tasks)
            {
                var timer = kvp.Value;
                timer.Time -= Time.deltaTime;
                if(timer.Time <= 0)
                {
                    timer.Callback();
                    toRemove.Add(kvp.Key);
                }
            }
            toRemove.ForEach(x => _tasks.Remove(x));
        }

    }
}
