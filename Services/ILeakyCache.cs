using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    // Ensures proper disposal patterns are used for buffer and memory management
    public class LeakyCache : ILeakyCache, IDisposable
    {
        private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>(); // Holds cached data
        private bool _disposed = false; // Tracks disposal state for cleanup logic

        // Task method to add data to cache
        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            // Validate size input
            if (sizeInMb <= 0)
            {
                throw new ArgumentException("Size must be greater than zero.");
            }

            // Avoid creating long-lived strings unnecessarily by scoping buffer creation and processing
            byte[] buffer = new byte[sizeInMb * 1024 * 1024];
            try
            {
                // Simulating processing delay
                await Task.Delay(50);

                // Safeguards logging long strings unless absolutely required for debugging
                string logMessage = $"Adding {sizeInMb}MB to cache for key: {key}";
                Console.WriteLine(logMessage); // Proper scoped lifetime for message

                // Adding processed buffer into the cache
                _cache[key] = buffer;

                return $"Added {sizeInMb}MB to cache with key: {key}";
            }
            catch 
            {
                // Free buffer on error case to prevent leak
                buffer = null;

                throw;
            }
        }


