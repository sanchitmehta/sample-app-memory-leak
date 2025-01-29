using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
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
        private readonly ConcurrentDictionary<string, byte[]> _cache = new ConcurrentDictionary<string, byte[]>();
        private readonly HttpClient _httpClient;

        public LeakyCache()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            // Allocate memory and perform HTTP request
            byte[] data = null;
            try
            {
                data = new byte[sizeInMb * 1024 * 1024]; 
                using (var response = await _httpClient.GetAsync("https://example.com"))
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    await stream.ReadAsync(data.AsMemory(0, data.Length));
                }
                _cache[key] = data;
            }
            catch (Exception)
            {
                data = null; 
                throw;
            }
            return key;
        }

        public int GetCacheSize()
        {
            long totalSize = 0;
            foreach (var item in _cache.Values)
            {
                totalSize += item.LongLength;
            }
            return (int)(totalSize / (1024 * 1024));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _cache.Clear();
        }
    }
}