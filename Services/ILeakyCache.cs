using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public interface ILeakyCache : IDisposable
    {
        Task<string> AddToCache(string key, int sizeInMb);
        int GetCacheSize();
    }

    public class LeakyCache : ILeakyCache
    {
        private readonly ConcurrentDictionary<string, byte[]> _cache = new ConcurrentDictionary<string, byte[]>();
        private bool _disposed = false;

        public Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));

            var sizeInBytes = sizeInMb * 1024 * 1024;
            var data = new byte[sizeInBytes]; // This might be the large byte[] issue

            // Just sample data, replace with actual logic
            for (int i = 0; i < sizeInBytes; i++)
            {
                data[i] = (byte)(i % 256);
            }

            _cache[key] = data;
            return Task.FromResult(key);
        }

        public int GetCacheSize()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));

            int totalSize = 0;
            foreach (var kvp in _cache)
            {
                totalSize += kvp.Value.Length;
            }
            return totalSize;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clear the cache to release the large byte arrays
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

        ~LeakyCache()
        {
            Dispose(false);
        }
    }
}