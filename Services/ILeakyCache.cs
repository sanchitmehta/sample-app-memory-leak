using System.IO;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public interface ILeakyCache : IDisposable
    {
        Task<string> AddToCache(string key, int sizeInMb);
        int GetCacheSize();
    }

    public sealed class LeakyCache : ILeakyCache
    {
        private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));

            byte[] data = new byte[sizeInMb * 1024 * 1024];
            using (var ms = new MemoryStream(data))
            {
                await ms.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                _cache[key] = data;
            }

            return key;
        }

        public int GetCacheSize()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache));

            return _cache.Values.Sum(v => v.Length);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _cache.Clear();
            }

            _disposed = true;
        }

        ~LeakyCache()
        {
            Dispose(false);
        }
    }
}