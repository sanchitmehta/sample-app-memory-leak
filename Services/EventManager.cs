using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Http; // Adding for HttpClient
using System.Text;

namespace PerformanceIssues.Services
{
    public class EventManager : IEventManager, IDisposable
    {
        private readonly List<WeakReference> _subscribers = new();
        private readonly List<Action<string>> _strongSubscribers = new(); // This will be removed to address intentional memory leak
        private readonly HttpClient _httpClient; // Example of managing HTTP connection properly
        private readonly Pipe _pipe; // Example for handling I/O pipelines

        private bool _disposed = false; // To track disposal

        public EventManager()
        {
            _httpClient = new HttpClient(); // Properly manage HttpClient
            _pipe = new Pipe(); // Initialize Pipe for example usage
        }

        public void Subscribe(Action<string> handler)
        {
            // Addressing issue by only storing a weak reference
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
            }
        }

        // Dispose pattern implementation to release unmanaged resources
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
                // Dispose the HttpClient to release HTTP connections
                _httpClient?.Dispose();

                // Complete and reset the pipe to ensure proper cleanup
                _pipe.Writer.Complete();
                _pipe.Reader.Complete();
            }

            _disposed = true;
        }

        ~EventManager()
        {
            // Finalizer to ensure cleanup of unmanaged resources
            Dispose(false);
        }
    }
}