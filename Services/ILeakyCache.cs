namespace PerformanceIssues.Services
{
    public interface ILeakyCache : IDisposable
    {
        Task<string> AddToCache(string key, int sizeInMb);
        int GetCacheSize();
    }
    
    public class LeakyCache : ILeakyCache
    {
        private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();

        public Task<string> AddToCache(string key, int sizeInMb)
        {
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _cache[key] = data;
            return Task.FromResult(key);
        }

        public int GetCacheSize()
        {
            return _cache.Values.Sum(arr => arr.Length) / (1024 * 1024);
        }

        public void Dispose()
        {
            ClearCache();
        }

        private void ClearCache()
        {
            _cache.Clear();
        }
    }
}