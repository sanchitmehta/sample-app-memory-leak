using System;
using System.Collections.Concurrent;
using System.Threading;
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
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(LeakyCache));
            }

            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _cache[key] = data;

            // Simulate async operation
            await Task.Delay(100, _cts.Token).ConfigureAwait(false);

            return key;
        }

        public int GetCacheSize()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(LeakyCache));
            }

            int size = 0;
            foreach (var item in _cache)
            {
                size += item.Value.Length;
            }
            return size;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                // Dispose of CancellationTokenSource
                _cts.Cancel();
                _cts.Dispose();

                // Clear the cache
                _cache.Clear();
            }
        }
    }
}