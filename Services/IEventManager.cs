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
        private bool _disposed;

        public void Subscribe(Action<string> handler)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EventManager));
            }
            
            _handlers.Add(handler);
        }

        public void RaiseEvent(string message)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EventManager));
            }
            
            foreach (var handler in _handlers)
            {
                handler?.Invoke(message);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _handlers.Clear(); // Clear the collection to release references
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }
    }
}