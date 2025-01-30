using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable // Added IDisposable for proper cleanup
    {
        private readonly List<WeakReference> _subscribers = new();
        private readonly List<Action<string>> _strongSubscribers = new();
        private bool _disposed = false; // Added disposed field for tracking disposal state

        public void Subscribe(Action<string> handler)
        {
            // Fix: Avoid storing both weak and strong references to the same handler
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
                    // Remove collected weak references to avoid memory leaks
                    _subscribers.Remove(weakRef);
                }
            }
        }

        public void Dispose()
        {
            // Dispose pattern to clean up resources
            if (!_disposed)
            {
                _subscribers.Clear(); // Clear the list of weak references
                _disposed = true;
            }
        }

        ~EventManager()
        {
            Dispose(); // Ensure cleanup happens in case Dispose is not called
        }
    }
}