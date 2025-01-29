using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public interface ILeakyCache
    {
        Task<string> AddToCache(string key, int sizeInMb);
        int GetCacheSize();
    }

    public class LeakyCache : ILeakyCache, IDisposable
    {
        private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));

            // Clear the same key if it already exists to avoid duplicate memory allocation
            if (_cache.ContainsKey(key))
            {
                _cache.Remove(key);
            }

            // Allocate memory equivalent to sizeInMb
            byte[] data;
            try
            {
                data = new byte[sizeInMb * 1024 * 1024];
            }
            catch (OutOfMemoryException)
            {
                return "Insufficient memory to allocate cache.";
            }

            // Simulate delay
            await Task.Delay(100);

            _cache[key] = data;
            return $"Cache item {key} with size {sizeInMb} MB added.";
        }

        public int GetCacheSize()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));

            int totalSize = 0;
            foreach (var item in _cache.Values)
            {
                totalSize += item.Length;
            }
            return totalSize / (1024 * 1024); // Convert to MB
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
                    // Clear cache to release allocated memory
                    _cache.Clear();
                }
                _disposed = true;
            }
        }

        ~LeakyCache()
        {
            Dispose(false);
        }
    }
}