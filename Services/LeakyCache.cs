using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        // Static fields replaced with instance fields to enable proper cleanup during disposal
        private readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private readonly Random _random = new();
        private bool _disposed = false; // Track disposal

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            // Intentionally creating large byte arrays and storing them indefinitely
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            // Simulate some async work
            await Task.Delay(100);

            // Ensure old data associated with the same key gets replaced and avoid indefinite growth
            if (_cache.ContainsKey(key))
            {
                if (_cache.TryRemove(key, out var oldData))
                {
                    // Explicitly release the replaced byte array
                    oldData = null;
                }
            }
            _cache.TryAdd(key, data);
            return key;
        }

        public int GetCacheSize()
        {
            CleanUpCache(); // Periodically clean up stale cache entries
            return _cache.Count;
        }

        private void CleanUpCache()
        {
            // Logic for cache cleanup can be expanded based on requirements (e.g., time-to-live expiration)
            var keysToRemove = _cache.Keys.Take(5).ToList(); // Simplistic cleanup strategy: remove first few items

            foreach (var key in keysToRemove)
            {
                if (_cache.TryRemove(key, out var oldData))
                {
                    // Explicitly release memory for cleanup items
                    oldData = null;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources
                    foreach (var key in _cache.Keys)
                    {
                        if (_cache.TryRemove(key, out var oldData))
                        {
                            oldData = null;
                        }
                    }
                }

                // Additional cleanup of unmanaged resources (if any)

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LeakyCache()
        {
            Dispose(false);
        }
    }
}