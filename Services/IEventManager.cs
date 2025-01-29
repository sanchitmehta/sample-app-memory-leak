using System;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public interface IEventManager : IDisposable
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }

    public class EventManager : IEventManager
    {
        private readonly List<Action<string>> _handlers = new List<Action<string>>();
        private bool _disposed = false;

        public void Subscribe(Action<string> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (_handlers)
            {
                _handlers.Add(handler);
            }
        }

        public void RaiseEvent(string message)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(EventManager));

            List<Action<string>> handlersSnapshot;
            lock (_handlers)
            {
                handlersSnapshot = new List<Action<string>>(_handlers);
            }

            foreach (var handler in handlersSnapshot)
            {
                handler.Invoke(message);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            lock (_handlers)
            {
                _handlers.Clear();
            }

            _disposed = true;
        }
    }
}