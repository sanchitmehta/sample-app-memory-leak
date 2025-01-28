using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private readonly Random _random = new();
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            // Creating large byte arrays
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            // Simulate some async work
            await Task.Delay(100);

            _cache.TryAdd(key, data);
            return key;
        }

        public int GetCacheSize() => _cache.Count;

        public void ClearCache()
        {
            _cache.Clear();
        }

        ~LeakyCache()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _cache.Clear();
                }

                // Free unmanaged resources (if any)

                _disposed = true;
            }
        }
    }
}