using System;
using System.IO;
using System.Net.Http; // Ensure HttpClient dependencies for demonstrating cleanup

namespace PerformanceIssues.Services
{
    public interface ICPUIntensiveTask
    {
        void Start();
        void Stop();
    }

    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        // Step 1: Add IDisposable implementation to allow cleanup of resources.
        private bool _disposed;

        // Step 2: Manage any memory-consuming resources (e.g., HttpClient or Stream).
        private HttpClient _httpClient;

        public CPUIntensiveTask()
        {
            // Initialize resources that are expensive and need proper cleanup.
            _httpClient = new HttpClient();
        }

        public void Start()
        {
            // Example of using an HttpClient, simulate heavy memory usage operations.
            try
            {
                // Use 'using' or proper async disposal if working with large data.
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, "http://example.com"))
                {
                    // Perform HTTP operations here.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Proper error handling.
            }
        }

        public void Stop()
        {
            // Ideally, manage any stop logic or cleanup here if required.
            Console.WriteLine("Task stopped.");
        }

        // Step 3: Implement Dispose method for proper resource cleanup.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free managed resources.
                    if (_httpClient != null)
                    {
                        _httpClient.Dispose();
                        _httpClient = null; // Clear reference.
                    }
                }

                // Free unmanaged resources if any (not used in this case).

                _disposed = true; // Mark as disposed.
            }
        }

        ~CPUIntensiveTask()
        {
            // Finalizer for unmanaged resources, not commonly needed anymore.
            Dispose(false);
        }
    }
}