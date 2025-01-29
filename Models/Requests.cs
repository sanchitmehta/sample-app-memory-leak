namespace PerformanceIssues.Models
{
    public class CacheEntryRequest : IDisposable
    {
        private bool _disposed;
        public int SizeMB { get; set; }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Example cleanup for CacheEntryRequest. If there were any large resources related to SizeMB, clean them here.
                // For now, no specific cleanup is needed.

                _disposed = true;
            }
        }
    }

    public class CPUTaskRequest : IDisposable
    {
        private bool _disposed;
        public int Complexity { get; set; }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Example cleanup for CPUTaskRequest. If there were complex computations, ensure any temporary resources are cleaned.
                _disposed = true;
            }
        }
    }

    public class DataGenerationRequest : IDisposable
    {
        private bool _disposed;
        public int RecordCount { get; set; }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Simulate clearing large collections or generated data tied to RecordCount.
                _disposed = true;
            }
        }
    }
}