using System.Collections.Concurrent; 
using System.Threading; 
using System.Threading.Tasks; 

namespace PerformanceIssues.Services 
{ 
    public class LeakyCache : ILeakyCache 
    { 
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new(); 
        private static readonly Random _random = new(); 
        private static readonly SemaphoreSlim _cacheLock = new(1, 1);

        public async Task<string> AddToCache(string key, int sizeInMb, CancellationToken cancellationToken) 
        { 
            await _cacheLock.WaitAsync(cancellationToken);
            try 
            { 
                // Use a maximum cache size limit to avoid unbounded growth
                const int maxCacheSize = 100; // Example max cache size limit 
                if (_cache.Count >= maxCacheSize) 
                { 
                    // Remove the oldest item
                    var oldestKey = _cache.Keys.OrderBy(k => k).FirstOrDefault();
                    if (oldestKey != null) 
                    { 
                        _cache.TryRemove(oldestKey, out _); 
                    } 
                }

                // Intentionally creating large byte arrays and storing them indefinitely 
                byte[] data = new byte[sizeInMb * 1024 * 1024]; 
                _random.NextBytes(data); 

                // Simulate some async work 
                await Task.Delay(100, cancellationToken);

                _cache[key] = data;
                return key; 
            } 
            finally 
            { 
                _cacheLock.Release(); 
            } 
        }

        public int GetCacheSize() => _cache.Count;

        // Method to clear the cache for resource cleanup
        public void ClearCache() 
        { 
            _cache.Clear(); 
        }

        // Use Dispose pattern for managing _cacheLock
        private bool disposed = false;
        public void Dispose() 
        { 
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) 
        { 
            if (!disposed) 
            { 
                if (disposing) 
                { 
                    _cacheLock.Dispose(); 
                }
                disposed = true;
            } 
        }

        ~LeakyCache() 
        { 
            Dispose(false);
        }
    }
}