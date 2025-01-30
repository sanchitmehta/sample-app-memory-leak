using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        // Using a dictionary to store large byte arrays; must ensure proper disposal of items when no longer needed.
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private static readonly Random _random = new();
        private bool _disposed = false; // Track disposable state.

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            // Intentionally creating large byte arrays (should scope their lifetime to avoid long-lived memory consumption)
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            // Simulate some async work
            await Task.Delay(100);

            // Ensure that we avoid indefinite storage of objects by removing keys if they already exist.
            // Could also add eviction policies if data growth is significant.
            if (!_cache.TryAdd(key, data))
            {
                // If the key already exists, free the created data array to prevent memory leaks.
                data = null; // Allow GC to reclaim memory.
            }

            return key;
        }

        public int GetCacheSize() => _cache.Count;

        // Implement IDisposable to clear unused resources.
        public void Dispose()
        {
            if (!_disposed)
            {
                // Ensure we clear the cache and free allocated memory properly.
                ClearCache();

                _disposed = true;
            }
        }

        // Helper method to clear the cache properly.
        private void ClearCache()
        {
            foreach (var key in _cache.Keys.ToList())
            {
                // Remove items from cache and clear allocated memory.
                if (_cache.TryRemove(key, out var data))
                {
                    data = null; // Allow GC to collect memory.
                }
            }
        }
    }
}