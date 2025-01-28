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
        private readonly List<Action<string>> _subscribers = new List<Action<string>>();
        private bool _disposed = false;

        public void Subscribe(Action<string> handler)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(EventManager));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _subscribers.Add(handler);
        }

        public void RaiseEvent(string message)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(EventManager));
            foreach (var subscriber in _subscribers)
            {
                subscriber?.Invoke(message);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            // Clear the subscribers list
            _subscribers.Clear();

            // Release any other resources here if necessary

            _disposed = true;
        }
    }
}