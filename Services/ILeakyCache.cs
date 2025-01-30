using System;
using System.Collections.Concurrent;
using System.IO;
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
        // ConcurrentDictionary to manage cache
        private readonly ConcurrentDictionary<string, byte[]> _cacheStorage = new ConcurrentDictionary<string, byte[]>();

        // Preventing unbounded memory growth: Add cleanup and proper disposal for resources
        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            // Validate inputs
            if (sizeInMb <= 0)
            {
                throw new ArgumentException("Size in MB must be greater than zero.", nameof(sizeInMb));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key must not be null or empty.", nameof(key));
            }

            // Properly dispose resources: Ensure memory is handled effectively
            byte[] data = null;
            try
            {
                data = new byte[sizeInMb * 1024 * 1024]; // Allocate memory
                for (int i = 0; i < data.Length; i++) // Simulate data population
                {
                    data[i] = 0xFF;
                }

                // Adding the data to the cache
                if (!_cacheStorage.TryAdd(key, data))
                {
                    throw new InvalidOperationException("Failed to add data to the cache.");
                }
            }
            catch (Exception ex) // Handle exceptions and release memory
            {
                Console.WriteLine($"Exception while adding to cache: {ex.Message}");
                if (data != null)
                {
                    data = null; // Nullify unused memory in case of failure
                }
                throw;
            }

            await Task.CompletedTask; // Simulate asynchronous work
            return $"Added {key} with size {sizeInMb} MB.";
        }

        // Optimize memory clean-up using proper disposal
        public int GetCacheSize()
        {
            // Calculate current cache size
            int totalSizeInMb = 0;
            foreach (var item in _cacheStorage)
            {
                if (item.Value != null)
                {
                    totalSizeInMb += item.Value.Length / (1024 * 1024); // Convert bytes to MB
                }
            }
            return totalSizeInMb;
        }

        // Proper IDisposable implementation to release unmanaged resources
        public void Dispose()
        {
            foreach (var key in _cacheStorage.Keys)
            {
                if (_cacheStorage.TryRemove(key, out var value))
                {
                    // Clear and release each cached byte array
                    if (value != null)
                    {
                        Array.Clear(value, 0, value.Length);
                    }
                }
            }

            // Clear cache store
            _cacheStorage.Clear();
        }
    }
}