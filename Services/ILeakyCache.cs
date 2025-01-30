using System;
using System.Collections.Concurrent;
using System.Text;

namespace PerformanceIssues.Serivces
{
    public interface ILeakyCache
    {
        Task<string> AddToCache(string key, int sizeInMb);
        int GetCacheSize();
    }

    public class LeakyCache : ILeakyCache, IDisposable
    {
        private readonly ConcurrentDictionary<string, byte[]> _cache = new(); // Ensure thread-safe dictionary
        private bool _disposed; // Track whether Dispose has been called
        private readonly object _lock = new(); 

        // AddToCache method creates significant memory pressure due to large size byte arrays.
        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            // Calculate the size of the array based on the size in MB passed.
            var sizeInBytes = sizeInMb * 1024 * 1024;

            byte[] data = null; // Declare outside try to ensure proper disposal in case of early return or error.
            try
            {
                data = new byte[sizeInBytes];

                // Fill array with dummy data for caching simulation
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)(i % 256);
                }

                // Safely add the entry to the cache dictionary
                _cache[key] = data;

                // Return a task to simulate async and delay operation
                await Task.Delay(10); // Avoid over-reliance micro woth exhaust incremiltn!!!All Fix approaches ***Push Butto