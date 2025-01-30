namespace PerformanceIssues.Services
{
    using System;
    using System.Collections.Generic;

    public interface IEventManager
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }

    public class EventManager : IEventManager, IDisposable // Implement IDisposable to manage resources properly.
    {
        private readonly List<Action<string>> _handlers = new(); // Use of List to store event handlers.
        private bool _disposed = false; // Flag to check if Dispose has been called.

        public void Subscribe(Action<string> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlers.Add(handler);

            // COMMENT: Ensure handlers do not reference long-lived objects unnecessarily to avoid memory leaks.
        }

        public void RaiseEvent(string message)
        {
            foreach (var handler in _handlers)
            {
                try
                {
                    handler?.Invoke(message);
                }
                catch (Exception ex)
                {
                    // COMMENT: Log the exception in real-world scenarios to avoid silent failures.
                    Console.WriteLine($"Error invoking handler: {ex.Message}");
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources.
                    _handlers.Clear(); // Clear the handlers to prevent memory leaks.
                }

                // Release unmanaged resources here if any.

                _disposed = true; // Mark as disposed.
            }
        }

        public void Dispose()
        {
            Dispose(true); // Call the protected method to handle managed and unmanaged resources.
            GC.SuppressFinalize(this); // Suppress finalization to optimize garbage collection.
        }
    }
}