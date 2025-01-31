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
        private readonly ConcurrentDictionary<string, byte[]> _cache = new ConcurrentDictionary<string, byte[]>();
        private bool _disposed = false; // Track whether Dispose has been called.

        // Adding proper async resource cleanup.
        public async Task<string> AddToCache(string key, int sizeInMb)
        {
            // Ensure size is non-negative (sanity check).
            if (sizeInMb <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeInMb), "Size must be positive.");
            }

            // Properly cleanup any used byte[] buffers if overwritten.
            if (_cache.ContainsKey(key))
            {
                if (_cache.TryRemove(key, out byte[] existingBuffer))
                {
                    CleanupBuffer(existingBuffer);
                }
            }

            // Use try-finally to ensure any exceptions during allocation do not cause resource leaks.
            byte[] buffer = null;
            try
            {
                buffer = new byte[sizeInMb * 1024 * 1024]; // Allocate buffer.
                // Simulate some work on the buffer.
                await Task.Run(() => Array.Clear(buffer, 0, buffer.Length));
                _cache[key] = buffer;
            }
            catch
            {
                // Cleanup in case of exception.
                if (buffer != null)
                {
                    CleanupBuffer(buffer);
                }
                throw;
            }

            return "Item added to cache successfully.";
        }

        public int GetCacheSize()
        {
            long totalSize = 0;
            foreach (var item in _cache)
            {
                totalSize += item.Value?.LongLength ?? 0;
            }
            return (int)(totalSize / (1024 * 1024)); // Return size in MB.
        }

        // Introduce a helper method to properly cleanup retained byte[] resources.
        private void CleanupBuffer(byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length); // Clear contents if needed for security purposes.
            buffer = null; // Ensure reference is released.
        }

        // Implementing IDisposable pattern to release resources properly.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return; // Avoid multiple Dispose calls.
            }

            if (disposing)
            {
                // Dispose of managed resources.
                foreach (var item in _cache)
                {
                    CleanupBuffer(item.Value);
                }
                _cache.Clear();
            }

            // Release unmanaged resources, if any, here.

            _disposed = true;
        }

        ~LeakyCache()
        {
            Dispose(false); // Finalizer releases unmanaged resources only.
        }
    }
}