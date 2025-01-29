using System.Collections.Concurrent;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private readonly Random _random = new();
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            await Task.Delay(100);

            _cache.TryAdd(key, data);
            return key;
        }

        public int GetCacheSize()
        {
            return _cache.Count;
        }

        public void ClearCache()
        {
            foreach (var key in _cache.Keys)
            {
                _cache.TryRemove(key, out _);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                ClearCache();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~LeakyCache()
        {
            Dispose();
        }
    }
}