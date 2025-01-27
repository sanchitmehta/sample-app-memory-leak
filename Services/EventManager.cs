using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference<Action<string>>> _subscribers = new();

        public void Subscribe(Action<string> handler)
        {
            _subscribers.Add(new WeakReference<Action<string>>(handler));
        }

        public void RaiseEvent(string message)
        {
            foreach (var weakRef in _subscribers.ToList())
            {
                if (weakRef.TryGetTarget(out var handler))
                {
                    handler(message);
                }
                else
                {
                    _subscribers.Remove(weakRef);
                }
            }
        }

        public void Dispose()
        {
            _subscribers.Clear();
        }
    }

    public interface IEventManager
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }
}