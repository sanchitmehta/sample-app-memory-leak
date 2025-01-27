using System;
using System.Collections.Concurrent;
using System.IO.Pipelines;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace PerformanceIssues.Services
{
    public interface IEventManager : IDisposable
    {
        void Subscribe(Action<string> handler);
        void RaiseEvent(string message);
    }

    public class EventManager : IEventManager
    {
        private readonly ConcurrentBag<Action<string>> _handlers = new ConcurrentBag<Action<string>>();
        private readonly Pipe _pipe = new Pipe();
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _disposed = false;

        public void Subscribe(Action<string> handler)
        {
            _handlers.Add(handler);
        }

        public void RaiseEvent(string message)
        {
            foreach (var handler in _handlers)
            {
                handler.Invoke(message);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _pipe.Writer.CompleteAsync().GetAwaiter().GetResult();
                _pipe.Reader.CompleteAsync().GetAwaiter().GetResult();
                _pipe.Reset();

                _httpClient.Dispose();
                _cancellationTokenSource.Dispose();

                // Clear the handlers collection
                _handlers.Clear();

                _disposed = true;
            }
        }
    }
}