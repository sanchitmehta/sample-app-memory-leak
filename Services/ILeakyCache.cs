using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    // Fix 1: Ensure proper disposal patterns and memory awareness for cached objects
    public class LeakyCache : ILeakyCache, IDisposable // Implement IDisposable to clean up resources
    {
        private readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private bool _disposed = false; // Flag to track disposal

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LeakyCache)); // Guard against usage after disposal

            // Fix 2: Use a using pattern for memory streams to avoid unmanaged resource leaks
            using (var memoryStream = new MemoryStream())
            {
                // Simulate adding a large byte array to the cache
                byte[] data = new byte[sizeInMb * 1024 * 1024]; // Allocate large byte array

                // Populate the array with dummy data asynchronously
                await memoryStream.WriteAsync(data, 0, data.Length);

                // Improvements: Cache should hold only lightweight references to avoid excessive memory retention
                _cache[key] = data;
            }

            // Return confirmation
            return $"{key} added to cache.";
        }

        public int GetCacheSize()
        {
            // Return size of the entire cache in MB
            int totalSizeInBytes = 0;

            foreach (var entry in _cache)
            {
                totalSizeInBytes += entry.Value.Length; // Calculate total size
            }
            return totalSizeInBytes / (1024 * 1024); // Convert to MB
        }

        // Implement Dispose to clean up unmanaged resources and prevent memory leaks
        public void Dispose()
        {
            if (_disposed) return;

            // Dispose logic for Byte[] cache structure
            _cache.Clear();
            
            // Ensure object is only disposed once
            _disposed = true;
        }
    }
}