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
        private readonly List<WeakReference> _resources = new List<WeakReference>();
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            var memoryStream = new MemoryStream();
            try
            {
                byte[] data = new byte[sizeInMb * 1024 * 1024];
                var random = new Random();
                random.NextBytes(data);

                memoryStream.Write(data, 0, data.Length);
                _cache[key] = data;
                _resources.Add(new WeakReference(memoryStream));

                return await Task.FromResult(key);
            }
            finally
            {
                memoryStream.Dispose();
            }
        }

        public int GetCacheSize()
        {
            int size = 0;
            foreach (var kvp in _cache)
            {
                size += kvp.Value.Length;
            }
            return size;
        }

        public void ClearCache()
        {
            _cache.Clear();
            _resources.Clear();
            GC.Collect();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var weakRef in _resources)
                    {
                        if (weakRef.IsAlive)
                        {
                            (weakRef.Target as IDisposable)?.Dispose();
                        }
                    }
                    _resources.Clear();
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