using System;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }
}

namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private List<byte[]> _largeCollections = new List<byte[]>();
        private bool _disposed = false;

        public void Start()
        {
            // Simulate CPU intensive task
            for (int i = 0; i < 100; i++)
            {
                _largeCollections.Add(new byte[1024 * 1024]); // Add 1 MB byte arrays
            }
        }

        public void Stop()
        {
            // Clear the large collection when no longer needed
            _largeCollections.Clear();
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
                    // Clear large collections to free memory
                    if (_largeCollections != null)
                    {
                        _largeCollections.Clear();
                        _largeCollections = null;
                    }
                    
                    // Unregister event handlers here if there are any
                    // e.g., MyEvent -= MyEventHandler;
                }

                // Free unmanaged resources here if there are any

                _disposed = true;
            }
        }

        ~CPUIntensiveTask()
        {
            Dispose(false);
        }
    }
}