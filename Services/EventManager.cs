using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();
        private readonly List<Action<string>> _strongSubscribers = new();
        private bool _disposed;

        public void Subscribe(Action<string> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _subscribers.Add(new WeakReference(handler));
        }

        public void RaiseEvent(string message)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EventManager));

            foreach (var weakRef in _subscribers.ToList())
            {
                if (weakRef.Target is Action<string> handler)
                {
                    handler(message);
                }
                else
                {
                    _subscribers.Remove(weakRef);
                }
            }
        }

        public void ClearSubscribers()
        {
            _subscribers.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _subscribers.Clear();
                    _strongSubscribers.Clear();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EventManager()
        {
            Dispose(false);
        }
    }
}