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
        private readonly List<Action<string>> _subscribers = new List<Action<string>>();
        private bool _disposed = false; // Track whether the object is disposed.

        // Subscribe allows registering handlers for the event. Ensure to clean up strong references.
        public void Subscribe(Action<string> handler)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EventManager));
            
            lock (_subscribers)
            {
                _subscribers.Add(handler);
            }
        }

        // RaiseEvent notifies all subscribers about the event, but the memory growth must be managed cautiously.
        public void RaiseEvent(string message)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EventManager));

            List<Action<string>> subscribersCopy;
            lock (_subscribers)
            {
                // Create a copy to prevent issues if the collection is modified while iterating.
                subscribersCopy = new List<Action<string>>(_subscribers);
            }

            foreach (var handler in subscribersCopy)
            {
                // Ensure proper error handling in case handlers throw exceptions.
                try
                {
                    handler(message);
                }
                catch (Exception ex)
                {
                    // Log the exception if a logging service is available.
                    // Avoid allowing exceptions in handlers to propagate.
                    Console.WriteLine($"Exception in event handler: {ex.Message}");
                }
            }
        }

        // Properly implement disposal pattern to avoid memory issues and circular references.
        public void Dispose()
        {
            if (!_disposed)
            {
                lock (_subscribers)
                {
                    // Ensure handlers are cleared to remove strong references to subscribed objects.
                    _subscribers.Clear();
                }

                _disposed = true;
            }
        }
    }
}