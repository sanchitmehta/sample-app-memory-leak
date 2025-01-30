using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        // ConcurrentDictionary to hold cached data - Ensure proper cleanup in Dispose method
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new();
        
        // Random instance to generate random byte arrays - No specific disposal needed as Random is not disposable
        private static readonly Random _random = new();
        
        // CancellationTokenSource to handle potential cancellation of async tasks - Properly dispose to release memory
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        private bool _disposed = false; // Track the disposal state

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));

            // Large byte arrays can cause memory issues if accumulated; ensure clear-up logic is in place
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            // Simulate some async work with a CancellationToken to avoid unnecessary memory retention
            try
            {
                await Task.Delay(100, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                // Handle task cancellation gracefully if necessary
                return string.Empty;
            }

            // Add data to cache
            _cache.TryAdd(key, data);
            return key;
        }

        public int GetCacheSize()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LeakyCache));

            return _cache.Count;
        }

        // Implement IDisposable to handle resource cleanup
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Suppress finalization as resources have been released explicitly
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Free up CancellationTokenSource resources
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                
                // Clear cache to release memory held by byte arrays
                _cache.Clear();
            }

            _disposed = true;
        }

        ~LeakyCache()
        {
            Dispose(false); // Ensure proper cleanup when object is finalized
        }
    }
}