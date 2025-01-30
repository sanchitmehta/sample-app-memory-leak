using System;
using System.Collections.Concurrent;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private static readonly Random _random = new();
        private bool _disposed = false; // Flag to track whether the object is disposed

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));

            // Creating large byte arrays, ensure we add proper cleanup in production scenarios
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            // Simulate some async work
            await Task.Delay(100);

            // Store data in the cache; consider strategies for expiring or removing old entries in long-running systems
            _cache.TryAdd(key, data);
            return key;
        }

        public int GetCacheSize()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));

            return _cache.Count;
        }

        // Implementing Dispose to handle proper cleanup
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                // Clear the cache to release memory held by byte arrays
                _cache.Clear();

                // Dispose further resources if needed in the future for extensibility
            }
        }
    }
}