namespace PerformanceIssues.Models
{
    using System;

    public class CacheEntryRequest : IDisposable
    {
        public int SizeMB { get; set; }
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            // Add cleanup logic if required, like clearing any large collections.
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~CacheEntryRequest()
        {
            Dispose();
        }
    }

    public class CPUTaskRequest : IDisposable
    {
        public int Complexity { get; set; }
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            // Add cleanup logic if required.
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~CPUTaskRequest()
        {
            Dispose();
        }
    }

    public class DataGenerationRequest : IDisposable
    {
        public int RecordCount { get; set; }
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            // Add cleanup logic if required.
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~DataGenerationRequest()
        {
            Dispose();
        }
    }
}