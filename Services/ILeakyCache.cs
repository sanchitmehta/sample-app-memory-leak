namespace PerformanceIssues.Services
{
    public interface ILeakyCache : IDisposable
    {
        Task<string> AddToCache(string key, int sizeInMb);
        int GetCacheSize();
    }
}

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache
    {
        private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (sizeInMb <= 0) throw new ArgumentOutOfRangeException(nameof(sizeInMb));

            byte[] data = new byte[sizeInMb * 1024 * 1024];
            lock (_cache)
            {
                _cache[key] = data;
            }

            await Task.CompletedTask;
            return key;
        }

        public int GetCacheSize()
        {
            lock (_cache)
            {
                return _cache.Sum(kv => kv.Value.Length);
            }
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
                    lock (_cache)
                    {
                        _cache.Clear();
                    }
                }
                _disposed = true;
            }
        }
    }
}