using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public interface ILeakyCache : IDisposable
    {
        Task<string> AddToCache(string key, int sizeInMb);
        int GetCacheSize();
    }

    public class LeakyCache : ILeakyCache
    {
        private readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private bool _disposed;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public LeakyCache()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(LeakyCache));
            }

            byte[] data = new byte[sizeInMb * 1024 * 1024];
            try
            {
                // Simulate work of filling the data
                await Task.Run(() => Array.Fill(data, (byte)1), _cancellationTokenSource.Token);
                _cache[key] = data;
            }
            catch (OperationCanceledException)
            {
                // Handle case where the operation is canceled
                return "Operation was canceled";
            }
            return key;
        }

        public int GetCacheSize()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(LeakyCache));
            }

            return _cache.Sum(entry => entry.Value.Length / (1024 * 1024));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cancellationTokenSource?.Cancel();
                    _cancellationTokenSource?.Dispose();

                    // Clear the cache and nullify the references to ensure no memory leaks
                    foreach (var key in _cache.Keys)
                    {
                        _cache.TryRemove(key, out _);
                    }

                    _cache.Clear();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~LeakyCache()
        {
            Dispose(disposing: false);
        }
    }
}