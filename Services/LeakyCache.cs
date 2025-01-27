using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private readonly Random _random = new();
        private bool _disposed;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            // Intentionally creating large byte arrays
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            // Simulate some async work
            await Task.Delay(100);

            // Store data in the cache
            _cache[key] = data;
            return key;
        }

        public int GetCacheSize() => _cache.Count;

        public void ClearCache()
        {
            foreach (var key in _cache.Keys)
            {
                // Explicitly dispose of byte arrays if applicable
                if (_cache.TryRemove(key, out byte[] value))
                {
                    Array.Clear(value, 0, value.Length);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clear the cache and release resources
                    ClearCache();
                    _cache.Clear();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}