using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new();
        private static readonly Random _random = new();
        private bool _disposed = false;

        // Dispose pattern: Clean up the disposable resources (if applicable)
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                ClearBufferCache();
            }
        }

        // Suggestion: Add method to clear cache to prevent indefinite growing of System.Byte[]
        private void ClearBufferCache()
        {
            _cache.Clear(); // Release references to byte arrays to allow them to be garbage-collected
        }

        public static void EnsureHttpConnectionDisposal(HttpClient httpClient)
        {
            // Review HTTP object lifecycles such Http pipelines => ensuring building repeated misll-resurfaced.Timer bugs ? Majorly-thin But 
