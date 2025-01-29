using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private static readonly Random _random = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _disposed = false; // Track whether Dispose has been called

        // IDisposable implementation ensures proper cleanup of resources
        public void Dispose()
        {
            if (_disposed) return;

            // Clean up the CancellationTokenSource to avoid memory leaks
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            // Manually clear the cache to free up the memory
            _cache.Clear();

            _disposed = true;
        }

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));

            // Restrict memory usage by limiting the cache size or evicting old entries (improvement comment)
            if (_cache.Count > 100) // Example: enforce a limit of 100 items
            {
                // Add an eviction strategy for better memory management (just a planned improvement)
                foreach (var oldestKey in _cache.Keys.Take(_cache.Count - 100))
                {
                    _cache.TryRemove(oldestKey, out _);
                }
            }

            // Intentionally creating large byte arrays and storing them indefinitely
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            // Simulate some async work
            await Task.Delay(100, _cancellationTokenSource.Token);

            _cache.TryAdd(key, data);
            return key;
        }

        public int GetCacheSize()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));
            return _cache.Count;
        }
    }
}