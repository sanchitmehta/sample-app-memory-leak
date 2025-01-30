using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private static readonly Random _random = new();

        // Avoid static instances for disposables like HttpClient to better manage resources
        private readonly HttpClient _httpClient = new();

        private bool _disposed;

        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            // Fix for byte array retention: Ensure removal of unused cache entries
            if (_cache.Count > 1000) // Arbitrary eviction strategy
            {
                foreach (var oldestKey in _cache.Keys.Take(500))
                {
                    _cache.TryRemove(oldestKey, out _);
                }
            }
            
            byte[] data = new byte[sizeInMb * 1024 * 04;