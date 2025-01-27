namespace PerformanceIssues.Services
{
    public interface ILeakyCache : IDisposable
    {
        Task<string> AddToCache(string key, int sizeInMb);
        int GetCacheSize();
        void ClearCache();
    }
}

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache
    {
        private readonly Dictionary<string, byte[]> _cache = new();
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));

            if (_cache.ContainsKey(key))
            {
                throw new ArgumentException("Key already exists in the cache.");
            }

            // Simulate adding data to cache
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            Random rnd = new Random();
            rnd.NextBytes(data);

            _cache.Add(key, data);
            return await Task.FromResult(key);
        }

        public int GetCacheSize()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));
            return _cache.Values.Sum(arr => arr.Length);
        }

        public void ClearCache()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));
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
                    _cache.Clear();
                }
                _disposed = true;
            }
        }
    }
}