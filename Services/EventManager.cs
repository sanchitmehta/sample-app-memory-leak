using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();

        // Removed strongSubscribers to address intentional memory leak.
        // Strong references prevent garbage collection, leading to memory leaks.

        private bool _disposed = false; // Flag to track disposal status

        public void Subscribe(Action<string> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _subscribers.Add(new WeakReference(handler));

            // Removed adding to _strongSubscribers to mitigate the memory leak.
        }

        public void RaiseEvent(string message)
        {
            // Filter valid subscribers and clean up stale or dead references.
            foreach (var weakRef in _subscribers.ToList())
            {
                if (weakRef.Target is Action<string> handler)
                {
                    handler(message);
                }
                else
                {
                    _subscribers.Remove(weakRef); // Remove stale references
                }
            }
        }

        // Proper disposal pattern
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
                // Release managed resources, such as clearing subscribers list.
                _subscribers.Clear();
            }

            // No unmanaged resources to release, but this is placeholder if needed.
            _disposed = true;
        }

        ~EventManager()
        {
            Dispose(false); // Finalizer releases unmanaged resources if any.
        }
    }
}