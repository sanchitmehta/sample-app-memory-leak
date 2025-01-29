using System;
using System.Threading;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    // Interface for Event Manager
    public interface IEventManager
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }

    // Implementation of IEventManager
    public class EventManager : IEventManager, IDisposable
    {
        // List to store event handlers
        private readonly List<Action<string>> _eventHandlers = new List<Action<string>>();

        // Fields related to the identified issues
        private CancellationTokenSource _cts; // Use a CancellationTokenSource which needs disposal
        private bool _isDisposed = false; // Track disposal state to avoid multiple disposals

        public EventManager()
        {
            _cts = new CancellationTokenSource(); // Initialize the CancellationTokenSource
        }

        // Subscribe a handler to the event
        public void Subscribe(Action<string> handler)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(EventManager));

            _eventHandlers.Add(handler);
        }

        // Raise the event to all subscribed handlers
        public void RaiseEvent(string message)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(EventManager));

            foreach (var handler in _eventHandlers)
            {
                try
                {
                    handler(message);
                }
                catch (Exception ex)
                {
                    // Log exceptions properly or handle them as needed
                    // Placeholder for logging
                    Console.WriteLine($"Handler failed: {ex.Message}");
                }
            }
        }

        // Recommended disposal pattern
        public void Dispose()
        {
            // Dispose method acts as a cleanup mechanism for unmanaged resources or memory leaks
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // Core of the Dispose pattern
        protected virtual void Dispose(bool disposing)
        {
            // If already disposed, avoid processing again
            if (_isDisposed) return;

            if (disposing)
            {
                // Dispose managed objects here

                // Dispose CancellationTokenSource to avoid memory leaks
                if (_cts != null)
                {
                    _cts.Dispose();
                    _cts = null;
                }

                // Clear the handler list to avoid keeping references alive
                _eventHandlers.Clear();
            }

            // Free unmanaged resources here if applicable

            // Mark the object as disposed
            _isDisposed = true;
        }

        ~EventManager()
        {
            // Finalizer to ensure cleanup in case Dispose is not called
            Dispose(disposing: false);
        }
    }
}

