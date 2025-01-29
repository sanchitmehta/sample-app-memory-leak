using System;

namespace PerformanceIssues.Models
{
    public class CacheEntryRequest : IDisposable
    {
        public int SizeMB { get; set; }

        private bool disposed = false;

        public void Dispose()
        {
            if (!disposed)
            {
                // Cleanup resources if needed
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }

    public class CPUTaskRequest : IDisposable
    {
        public int Complexity { get; set; }

        private bool disposed = false;

        public void Dispose()
        {
            if (!disposed)
            {
                // Cleanup resources if needed
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }

    public class DataGenerationRequest : IDisposable
    {
        public int RecordCount { get; set; }

        private bool disposed = false;

        public void Dispose()
        {
            if (!disposed)
            {
                // Cleanup resources if needed
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}