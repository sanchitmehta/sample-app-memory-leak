using System;
using System.IO;
using System.Net.Http;

namespace PerformanceIssues.Models
{
    public class CacheEntryRequest
    {
        public int SizeMB { get; set; }

        // Consider if buffers related to SizeMB are being correctly disposed elsewhere in the application.
        // If allocated buffers for caching are used with System.Byte[], ensure proper usage in conjunction with MemoryStream or other IDisposable resources.
    }

    public class CPUTaskRequest
    {
        public int Complexity { get; set; }

        // Evaluate if CPUTaskRequest contributes to string interning or excessive retention of System.String.
        // Ensure transient strings are not stored in static fields inadvertently.
    }

    public class DataGenerationRequest : IDisposable
    {
        public int RecordCount { get; set; }

        // Adding IDisposable to enforce cleanup of related resource objects.
        private MemoryStream _bufferStream;
        private bool _disposed;

        public DataGenerationRequest()
        {
            // Example: Initiate a MemoryStream or similar resource to mimic potential real-world resource usage.
            _bufferStream = new MemoryStream();
        }

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
                    // Free managed resources here.
                    _bufferStream?.Dispose();
                }

                // Free unmanaged resources here if necessary.
                _disposed = true;
            }
        }

        ~DataGenerationRequest()
        {
            Dispose(false);
        }

        // Review any potential System.Byte[] retention here in conjunction with buffer streams.
        // Ensure buffers allocated/deallocated properly when generating data by RecordCount.
    }

    // General suggestion: 
    // Ensure proper disposal of HttpClient, Http1Connection, and HttpRequestHeaders outside of this scope, 
    // where HTTP requests are initiated. Use 'using' statements or implement a dedicated Dispose pattern.
}