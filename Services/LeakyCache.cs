using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private static readonly Random _random = new();
        private readonly CancellationTokenSource _cts = new();

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            await Task.Delay(100, _cts.Token);

            _cache.TryAdd(key, data);
            return key;
        }

        public int GetCacheSize() => _cache.Count;

        public void ClearCache()
        {
            _cache.Clear();
        }

        // Proper resource cleanup and disposal
        public void Dispose()
        {
            ClearCache();
            _cts.Cancel();
            _cts.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}