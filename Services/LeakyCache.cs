using System.Collections.Concurrent;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache
    {
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private static readonly Random _random = new();
        
        // Add a method to clear the cache manually when needed
        public void ClearCache()
        {
            _cache.Clear();
        }

        // This method was causing retention of large byte arrays
        // Fix: Scope the lifetime of byte arrays to prevent indefinite retention in memory
        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (sizeInMb <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeInMb), "Size must be greater than zero.");
            }

            // Ensure proper scope of the byte array
            byte[] data = null;

            try
            {
                data = new byte[sizeInMb * 1024 * 1024];
                _random.NextBytes(data);

                // Simulate some async work
                await Task.Delay(100);

                // Avoid adding null arrays to the cache
                _cache.TryAdd(key, data);
            }
            finally
            {
                // Explicitly set the reference to null after adding to the cache
                data = null;
                // Garbage collector will reclaim this memory eventually
            }

            return key;
        }

        public int GetCacheSize()
        {
            // Return the number of cached items
            return _cache.Count;
        }

        // Additional improvement: Consider logging cache usage statistics in a real-world scenario
    }
}