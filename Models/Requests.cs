using System;
using System.Net.Http;

namespace PerformanceIssues.Models
{
    public class CacheEntryRequest : IDisposable
    {
        public int SizeMB { get; set; }

        // Implement IDisposable to ensure proper cleanup of resources if needed
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources here
                }

                // Cleanup unmanaged resources here
                disposed = true;
            }
        }

        ~CacheEntryRequest()
        {
            Dispose(false);
        }
    }

    public class CPUTaskRequest : IDisposable
    {
        public int Complexity { get; set; }
        
        // Implement IDisposable to ensure proper cleanup of resources if needed
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources here
                }

                // Cleanup unmanaged resources here
                disposed = true;
            }
        }

        ~CPUTaskRequest()
        {
            Dispose(false);
        }
    }

    public class DataGenerationRequest : IDisposable
    {
        public int RecordCount { get; set; }

        // In the context of big data or large collections, ensure proper disposal
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources here
                }

                // Cleanup unmanaged resources here
                disposed = true;
            }
        }

        ~DataGenerationRequest()
        {
            Dispose(false);
        }
    }
}