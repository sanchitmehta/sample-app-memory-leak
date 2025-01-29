using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private readonly ConcurrentDictionary<string, byte[]> _cache = new ConcurrentDictionary<string, byte[]>();

        // CancellationTokenSource is disposable, we need to properly dispose it
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private bool _disposed = false;

        public Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));

            // Allocate a byte array to fill the memory, but be cautious about clearing it when necessary
            var data = new byte[sizeInMb * 1024 * 1024];

            // Add the data to the cache
            if (!_cache.TryAdd(key, data))
            {
                // Free up resources if the key already exists
                data = null;
            }

            // Scope improvements: Use Task.CompletedTask when no actual async computation is done
            return Task.FromResult("Added to cache");
        }

        public int GetCacheSize()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));

            // Calculate the total cache size
            return _cache.Values.Sum(v => v.Length);
        }

        // Properly implement IDisposable to dispose of managed resources
        public void Dispose()
        {
            if (_disposed)
                return;

            // Invalidate and dispose CancellationTokenSource
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            // Clear the cache and release memory held by byte arrays
            foreach (var key in _cache.Keys)
            {
                _cache.TryRemove(key, out var _); // Explicitly remove items to ensure proper cleanup
            }

            // Indicate this object is disposed
            _disposed = true;

            GC.SuppressFinalize(this); // No need for finalizer as disposal is explicit
        }

        // Optional but good practice: Finalizer to ensure no unmanaged resources are left undisposed
        ~LeakyCache()
        {
            Dispose();
        }
    }
}