using System;
using System.Collections.Generic;
using System.Threading;

namespace PerformanceIssues.Services
{
    public interface IEventManager : IDisposable
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }

    public class EventManager : IEventManager
    {
        private readonly List<Action<string>> _handlers;
        private bool _disposed;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public EventManager()
        {
            _handlers = new List<Action<string>>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

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
            List<Action<string>> handlersCopy;
            lock (_handlers)
            {
                handlersCopy = new List<Action<string>>(_handlers);
            }
            foreach (var handler in handlersCopy)
            {
                try
                {
                    handler?.Invoke(message);
                }
                catch
                {
                    // Log/handle exceptions as necessary
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            lock (_handlers)
            {
                _handlers.Clear();
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}