using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();
        private bool _disposed = false;

        public void Subscribe(Action<string> handler)
        {
            // Use only weak references to allow garbage collection when no strong references exist
            _subscribers.Add(new WeakReference(handler));
        }

        public void RaiseEvent(string message)
        {
            foreach (var weakRef in _subscribers.ToList())
            {
                if (weakRef.Target is Action<string> handler)
                {
                    handler(message);
                }
                else
                {
                    // Remove dead references
                    _subscribers.Remove(weakRef);
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Clear the collection to remove references
                _subscribers.Clear();
                _disposed = true;
            }
        }
    }
}