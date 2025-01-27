using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();
        private bool _disposed;

        public void Subscribe(Action<string> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            // Only keep a weak reference to avoid memory leaks
            _subscribers.Add(new WeakReference(handler));
        }

        public void RaiseEvent(string message)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(EventManager));

            foreach (var weakRef in _subscribers.ToList())
            {
                if (weakRef.Target is Action<string> handler)
                {
                    handler(message);
                }
                else
                {
                    // Remove dead references
                    _subscribers.Remove(weakRef);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _subscribers.Clear();
            _disposed = true;
        }
    }
}