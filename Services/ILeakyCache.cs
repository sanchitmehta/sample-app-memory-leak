using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
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
        private readonly ConcurrentDictionary<string, byte[]> _cache = new ConcurrentDictionary<string, byte[]>();
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));

            await _cacheLock.WaitAsync();
            try
            {
                // Dispose the previous value if it exists to avoid memory leaks
                if (_cache.TryGetValue(key, out var existingValue))
                {
                    _cache.TryRemove(key, out _);
                    Array.Clear(existingValue, 0, existingValue.Length);
                }

                var data = new byte[sizeInMb * 1024 * 1024];
                _cache[key] = data;

                return key;
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        public int GetCacheSize()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));

            var size = 0;
            foreach (var kvp in _cache)
            {
                size += kvp.Value.Length;
            }
            return size;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _cacheLock.Dispose();

            foreach (var key in _cache.Keys)
            {
                if (_cache.TryRemove(key, out var buffer))
                {
                    Array.Clear(buffer, 0, buffer.Length);
                }
            }
        }
    }
}