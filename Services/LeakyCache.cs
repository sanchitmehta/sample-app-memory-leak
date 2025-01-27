using System.Collections.Concurrent;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private readonly Random _random = new();
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));
            
            // Intentionally creating large byte arrays and storing them indefinitely
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            // Simulate some async work
            await Task.Delay(100);

            _cache.TryAdd(key, data);

            return key;
        }

        public int GetCacheSize()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));
                
            return _cache.Count;
        }

        public void ClearCache()
        {
            _cache.Clear();
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
                    ClearCache();
                }

                _disposed = true;
            }
        }
    }
}