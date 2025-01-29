using System.Collections.Concurrent;
using System.Net.Http;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private static readonly Random _random = new();
        private readonly HttpClient _httpClient = new();

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            // Ensure byte arrays are properly cleared when replaced or not used
            if (_cache.TryGetValue(key, out var existingData))
            {
                Array.Clear(existingData, 0, existingData.Length);
            }

            // Create and populate large byte arrays
            byte[] data = new byte[sizeInMb * 1024 * 1024];
            _random.NextBytes(data);

            // Simulate some async work
            await Task.Delay(100);

            // Add or replace the existing cache entry
            _cache[key] = data;

            return key;
        }

        public int GetCacheSize() => _cache.Count;

        public void ClearCache()
        {
            foreach (var key in _cache.Keys)
            {
                if (_cache.TryRemove(key, out var data))
                {
                    Array.Clear(data, 0, data.Length); // Clear memory explicitly
                }
            }
        }

        public void Dispose()
        {
            ClearCache(); // Free cache resources

            // Explicitly dispose of unmanaged resources
            _httpClient.Dispose();
        }
    }
}