using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
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
        private readonly object _lock = new object();
        private bool _disposed = false;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (string.IsNullOrEmpty(key) || sizeInMb <= 0)
                throw new ArgumentException("Invalid key or size");

            string result;
            var sizeInBytes = sizeInMb * 1024 * 1024;

            byte[] data = new byte[sizeInBytes];
            lock (_lock)
            {
                _cache[key] = data;
            }

            result = GenerateStringData(sizeInBytes);
            data = null; // Allow the data array to be eligible for garbage collection

            await Task.CompletedTask; // Simulate async operation
            return result;
        }

        public int GetCacheSize()
        {
            lock (_lock)
            {
                return _cache.Values.Sum(data => data.Length / (1024 * 1024)); // Return size in MB
            }
        }

        private string GenerateStringData(int sizeInBytes)
        {
           return!