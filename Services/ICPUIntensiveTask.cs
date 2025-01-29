namespace PerformanceIssues.Services
{
    using System;
    using System.Collections.Generic;

    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }

    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private bool _disposed = false;
        private List<byte[]> _largeDataCollections = new List<byte[]>();
        private SomeHttpClient _httpClient;

        public CPUIntensiveTask()
        {
            _httpClient = new SomeHttpClient(); // Assume this simulates an HTTP client.
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));

            // Simulate adding data to the list
            _largeDataCollections.Add(new byte[1024 * 1024]); // 1 MB object
        }

        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));

            // Perform necessary cleanup
            _largeDataCollections.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _largeDataCollections.Clear();
                    _largeDataCollections = null;

                    if (_httpClient != null)
                    {
                        _httpClient.Dispose();
                        _httpClient = null;
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Suppress finalization as cleanup is done
        }

        ~CPUIntensiveTask()
        {
            Dispose(false);
        }
    }

    public class SomeHttpClient : IDisposable
    {
        public void Dispose()
        {
            // Simulate releasing HTTP resources
        }
    }
}