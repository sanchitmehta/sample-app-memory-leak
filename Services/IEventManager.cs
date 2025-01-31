using System;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public interface IEventManager : IDisposable // Step 1: Implement IDisposable to ensure resources are released properly.
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }

    public class EventManager : IEventManager
    {
        private readonly List<Action<string>> _handlers = new List<Action<string>>(); // Step 2: Retain handlers in a List for managing subscriptions.
        private bool _disposed = false; // Step 3: Track disposal to prevent multiple disposals or use-after-free scenarios.

        public void Subscribe(Action<string> handler)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(EventManager)); // Step 4: Check if the object is already disposed to prevent invalid usage.
            if (handler == null) throw new ArgumentNullException(nameof(handler)); // Ensure handlers are non-null to avoid unexpected runtime exceptions.

            _handlers.Add(handler);
        }

        public void RaiseEvent(string message)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(EventManager)); // Step 4: Check disposal state before performing actions to prevent errors.

            foreach (var handler in _handlers)
            {
                try
                {
                    handler(message); // Step 5: Call subscribed handlers safely.
                }
                catch (Exception ex)
                {
                    // Step 6: Consider logging the exception if using a logger to improve observability. Uncomment the line below if a logger is available.
                    // Logger.LogError(ex, "Error invoking handler.");
                }
            }
        }

        public void Dispose() // Step 7: Implement IDisposable pattern correctly.
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _handlers.Clear(); // Step 8: Clear the handler list to release references and help with garbage collection.
                }

                _disposed = true;
            }
        }
    }
}

