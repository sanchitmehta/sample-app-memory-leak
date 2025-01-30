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
        private bool _isDisposed;

        public void Subscribe(Action<string> handler)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(EventManager));

            // Add handler to the list
            _handlers.Add(handler);
        }

        public void RaiseEvent(string message)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(EventManager));

            foreach (var handler in _handlers)
            {
                try
                {
                    // Invoke each handler
                    handler.Invoke(message);
                }
                catch (Exception)
                {
                    // Log exception or handle appropriately here
                }
            }
        }

        // Properly implement the IDisposable pattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected Dispose method for cleanup
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                // Release any resources if needed
                _handlers.Clear();
            }
            _isDisposed = true;
        }

        ~EventManager()
        {
            Dispose(false);
        }
    }
}