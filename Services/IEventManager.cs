namespace PerformanceIssues.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.Http;
    using System.Threading;

    public interface IEventManager
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }

    public class EventManager : IEventManager, IDisposable
    {
        private readonly ConcurrentBag<Action<string>> _handlers;
        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cancellationTokenSource; // Use one centralized CancellationTokenSource properly
        private bool _disposed;

        public EventManager()
        {
            _handlers = new ConcurrentBag<Action<string>>();
            _httpClient = new HttpClient(); // HttpClient should be disposed properly
            _cancellationTokenSource = new CancellationTokenSource(); // Ensure proper token disposal
        }

        public void Subscribe(Action<string> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlers.Add(handler);
        }

        public void RaiseEvent(string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            foreach (var handler in _handlers)
            {
                try
                {
                    handler(message);
                }
                catch (Exception ex)
                {
                    // Consider logging exceptions properly without retaining large objects unnecessarily
                }
            }

            // Simulated HTTP usage - ensure no unnecessary large byte arrays
            // Use async Dispose pattern properly to prevent resource leaks in newer environments
            using (var content = new StringContent(message))
            {
                var response = _httpClient.PostAsync("https://example.com", content).Result;
                // Ensure no memory is unnecessarily retained after completing the request
                var responseBody = response.Content.ReadAsStringAsync().Result; 
            }
        }

        // Dispose pattern to clean up resources properly
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Dispose managed resources like HttpClient and CancellationTokenSource
                _httpClient.Dispose();
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel(); // Cancel any pending operations to release resources
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }

            _disposed = true;
        }
    }
}