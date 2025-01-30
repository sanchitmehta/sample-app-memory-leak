using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceIssues.Services
{
    public interface IEventManager
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }

    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<Action<string>> _handlers = new List<Action<string>>();
        private bool _isDisposed; // Track disposal to prevent memory leaks

        public void Subscribe(Action<string> handler)
        {
            if (_isDisposed) // Ensure no action is taken after disposal
                throw new ObjectDisposedException(nameof(EventManager));

            // Using a lock to prevent thread safety issues in handler registration
            lock (_handlers)
            {
                _handlers.Add(handler);
            }
        }

        public void RaiseEvent(string message)
        {
            if (_isDisposed) // Prevent actions on disposed objects
                throw new ObjectDisposedException(nameof(EventManager));

            // Avoid creating long-lived strings unnecessarily
            // Instead of logging or retaining heavy messages in memory, immediately clear them
            string eventMessage = message; 

            lock (_handlers)
            {
                foreach (var handler in _handlers)
                {
                    handler(eventMessage);
                }
            }
        }

        

Cleanup unclosed handlers