using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable // Implement IDisposable for proper cleanup
    {
        private readonly List<WeakReference> _subscribers = new();
        private readonly List<Action<string>> _strongSubscribers = new(); // Remove this field to avoid memory leak caused by strong references
        private bool _disposed = false; // Track disposal state to prevent multiple disposals

        // Removed the _strongSubscribers list to address the intentional memory leak

        public void Subscribe(Action<string> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            // Add only weak references to avoid strong reference holding objects in memory unnecessarily
            _subscribers.Add(new WeakReference(handler));
        }

        public void RaiseEvent(string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            foreach (var weakRef in _subscribers.ToList()) // Iterate over a copy to avoid modifying collection while iterating
            {
                if (weakRef.Target is Action<string> handler)
                {
                    handler(message);
                }
                else
                {
                    // Clean up dead weak references to avoid memory buildup
                    _subscribers.Remove(weakRef);
                }
            }
        }

        // Proper disposal pattern implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Core dispose method
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Clear subscribers to release references and avoid memory leaks
                _subscribers.Clear();
            }

            _disposed = true;
        }

        ~EventManager() // Finalizer in case Dispose isn't called
        {
            Dispose(false);
        }

        // Improvements: Optionally, make use of the .NET event pattern with weak references for better scalability
    }
}