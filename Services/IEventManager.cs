namespace PerformanceIssues.Services
{
    using System;
    using System.Collections.Generic;

    public interface IEventManager : IDisposable
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }

    public class EventManager : IEventManager
    {
        private readonly List<Action<string>> _subscribers;
        private bool _disposed;

        public EventManager()
        {
            _subscribers = new List<Action<string>>();
        }

        public void Subscribe(Action<string> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (!_subscribers.Contains(handler))
            {
                _subscribers.Add(handler);
            }
        }

        public void RaiseEvent(string message)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EventManager));

            foreach (var subscriber in _subscribers)
            {
                subscriber?.Invoke(message);
            }
        }

        ~EventManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Clear event handlers to prevent memory leaks.
                _subscribers.Clear();
            }

            _disposed = true;
        }
    }
}